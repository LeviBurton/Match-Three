using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Board : MonoBehaviour
{
    public int width;
    public int height;
    public int borderSize;

    public GameObject tilePrefab;
    public GameObject[] gamePiecePrefabs;
    public float swapTime = 0.5f;

    GamePiece[,] m_allGamePieces;
    Tile[,] m_allTiles;

    Tile m_clickedTile;
    Tile m_targetTile;

    void Start()
    {
        m_allTiles = new Tile[width, height];
        m_allGamePieces = new GamePiece[width, height];

        SetupTiles();
        SetupCamera();
        FillBoard();
        HighlightMatches();
    }

    void SetupTiles()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GameObject tile = Instantiate(tilePrefab, new Vector3(i, j, 0), Quaternion.identity) as GameObject;
                tile.name = "Tile [" + i + "," + j + "]";
                m_allTiles[i, j] = tile.GetComponent<Tile>();
                tile.transform.parent = transform;

                m_allTiles[i, j].Init(i, j, this);
            }
        }
    }

    void SetupCamera()
    {
        Camera.main.transform.position = new Vector3((float)(width - 1) / 2f, (float)(height - 1) / 2f, -10f);

        float aspetRatio = (float)Screen.width / (float)Screen.height;
        float verticalSize = (float)height / 2f + (float)borderSize;
        float horizontalSize = (float)width / 2f + (float)borderSize / aspetRatio;

        Camera.main.orthographicSize = (verticalSize > horizontalSize) ? verticalSize : horizontalSize;
    }

    void FillBoard()
    {
        int maxIterations = 100;
        int iterations = 0;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                iterations = 0;
                var piece = FillRandomAt(i, j);

                while (HasMatchOnFill(i, j))
                {
                    ClearPieceAt(i, j);
                    piece = FillRandomAt(i, j);
                    iterations++;

                    if (iterations > maxIterations)
                        break;
                }
            }
        }
    }

    bool HasMatchOnFill(int x, int y, int minLength = 3)
    {
        var leftMatches = FindMatches(x, y, Vector3.left, minLength);
        var downwardMatches = FindMatches(x, y, Vector3.down, minLength);

        if (leftMatches == null)
            leftMatches = new List<GamePiece>();

        if (downwardMatches == null)
            downwardMatches = new List<GamePiece>();

        return (leftMatches.Any() || downwardMatches.Any());
    }

    GamePiece FillRandomAt(int x, int y)
    {
        GameObject randomPiece = Instantiate(GetRandomGamePiece(), Vector3.zero, Quaternion.identity) as GameObject;

        if (randomPiece != null)
        {
            var gamePieceComponent = randomPiece.GetComponent<GamePiece>();
            gamePieceComponent.Init(this);
            PlaceGamePiece(gamePieceComponent, x, y);
            randomPiece.transform.parent = transform;
            return gamePieceComponent;
        }
        return null;
    }

    GameObject GetRandomGamePiece()
    {
        int randomIdx = Random.Range(0, gamePiecePrefabs.Length);

        if (gamePiecePrefabs[randomIdx] == null)
        {
            Debug.LogWarning("Error: " + randomIdx + " does not have a prefab.");
        }

        return gamePiecePrefabs[randomIdx];
    }

    public void PlaceGamePiece(GamePiece gamePiece, int x, int y)
    {
        gamePiece.transform.position = new Vector3(x, y, 0);
        gamePiece.transform.rotation = Quaternion.identity;

        if (IsWithinBounds(x, y))
        {
            m_allGamePieces[x, y] = gamePiece;
        }
      
        gamePiece.SetCoord(x, y);
    }

    bool IsWithinBounds(int x, int y)
    {
        return (x >= 0 && x < width && y >= 0 && y < height);
    }

    public void ClickTile(Tile tile)
    {
        if (m_clickedTile == null)
        {
            m_clickedTile = tile;
        }
    }

    public void DragToTile(Tile tile)
    {
        if (m_clickedTile != null && IsNextTo(tile, m_clickedTile))
        {
            m_targetTile = tile;
        }
    }

    public void ReleaseTile()
    {
        if (m_clickedTile != null && m_targetTile != null)
        {
            SwitchTiles(m_clickedTile, m_targetTile);
        }

        m_clickedTile = null;
        m_targetTile = null;
    }

    void SwitchTiles(Tile clickedTile, Tile targetTile)
    {
        StartCoroutine(SwitchTilesRoutine(clickedTile, targetTile));
    }

    IEnumerator SwitchTilesRoutine(Tile clickedTile, Tile targetTile)
    {
        GamePiece clickedPiece = m_allGamePieces[clickedTile.xIndex, clickedTile.yIndex];
        GamePiece targetPiece = m_allGamePieces[targetTile.xIndex, targetTile.yIndex];

        if (targetPiece != null && clickedPiece != null)
        {
            clickedPiece.Move(targetTile.xIndex, targetTile.yIndex, swapTime);
            targetPiece.Move(clickedTile.xIndex, clickedTile.yIndex, swapTime);

            yield return new WaitForSeconds(swapTime);

            var clickedPieceMatches = FindMatchesAt(clickedTile.xIndex, clickedTile.yIndex);
            var targetPieceMatches = FindMatchesAt(targetTile.xIndex, targetTile.yIndex);

            if (targetPieceMatches.Count == 0 && clickedPieceMatches.Count == 0)
            {
                clickedPiece.Move(clickedTile.xIndex, clickedTile.yIndex, swapTime);
                targetPiece.Move(targetTile.xIndex, targetTile.yIndex, swapTime);
            }
            else
            {
                yield return new WaitForSeconds(swapTime);
                ClearPieceAt(clickedPieceMatches);
                ClearPieceAt(targetPieceMatches);
            }
        }
    }

    bool IsNextTo(Tile start, Tile end)
    {
        if (Mathf.Abs(start.xIndex - end.xIndex) == 1 && start.yIndex == end.yIndex)
        {
            return true;
        }

        if (Mathf.Abs(start.yIndex - end.yIndex) == 1 && start.xIndex == end.xIndex)
        {
            return true;
        }

        return false;
    }

    List<GamePiece> FindMatches(int startX, int startY, Vector2 searchDirection, int minLength = 3)
    {
        List<GamePiece> matches = new List<GamePiece>();

        GamePiece startPiece = null;

        if (IsWithinBounds(startX, startY))
        {
            startPiece = m_allGamePieces[startX, startY];
        }

        if (startPiece != null)
        {
            matches.Add(startPiece);
        }
        else
        {
            return null;
        }

        int nextX;
        int nextY;
        int maxValue = (width > height) ? width : height;

        for (int i = 1; i < maxValue - 1; i++)
        {
            nextX = startX + (int)Mathf.Clamp(searchDirection.x, -1, 1) * i;
            nextY = startY + (int)Mathf.Clamp(searchDirection.y, -1, 1) * i;

            if (!IsWithinBounds(nextX, nextY))
            {
                break;
            }

            GamePiece nextPiece = m_allGamePieces[nextX, nextY];

            if (nextPiece == null)
                break;
            else
            {
                if (nextPiece.matchValue == startPiece.matchValue && !matches.Contains(nextPiece))
                {
                    matches.Add(nextPiece);
                }
                else
                {
                    break;
                }
            }
        }

        if (matches.Count >= minLength)
        {
            return matches;
        }
        else
        {
            return null;
        }
    }

    List<GamePiece> FindVerticalMatches(int startX, int startY, int minLength = 3)
    {
        List<GamePiece> upwardMatches = FindMatches(startX, startY, Vector2.up, 2);
        List<GamePiece> downwaredMatches = FindMatches(startX, startY, Vector2.down, 2);

        if (upwardMatches == null)
        {
            upwardMatches = new List<GamePiece>();
        }

        if (downwaredMatches == null)
        {
            downwaredMatches = new List<GamePiece>();
        }

        var combinedMatches = upwardMatches.Union(downwaredMatches).ToList();

        return (combinedMatches.Count >= minLength) ? combinedMatches : null;
    }

    List<GamePiece> FindHorizontalMatches(int startX, int startY, int minLength = 3)
    {
        List<GamePiece> rightMatches = FindMatches(startX, startY, Vector2.right, 2);
        List<GamePiece> leftMatches = FindMatches(startX, startY, Vector2.left, 2);

        if (rightMatches == null)
        {
            rightMatches = new List<GamePiece>();
        }

        if (leftMatches == null)
        {
            leftMatches = new List<GamePiece>();
        }

        var combinedMatches = rightMatches.Union(leftMatches).ToList();

        return (combinedMatches.Count >= minLength) ? combinedMatches : null;
    }

    void HighlightMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                HighlightMatchesAt(i, j);
            }
        }
    }

    private void HighlightMatchesAt(int x, int y)
    {
        
        var combinedMatches = FindMatchesAt(x, y);

        if (combinedMatches.Count > 0)
        {
            foreach (var piece in combinedMatches)
            {
                HighlightTileOn(piece.xIndex, piece.yIndex, piece.GetComponent<SpriteRenderer>().color);
            }
        }
    }

    void HighlightTileOff(int x, int y)
    {
        var spriterRenderer = m_allTiles[x, y].GetComponent<SpriteRenderer>();
        spriterRenderer.color = new Color(spriterRenderer.color.r, spriterRenderer.color.g, spriterRenderer.color.b, 0);
    }

    void HighlightTileOn(int x, int y, Color col)
    {
        var spriterRenderer = m_allTiles[x, y].GetComponent<SpriteRenderer>();
        spriterRenderer.color = col;
    }

    List<GamePiece> FindMatchesAt(int x, int y, int minLength = 3)
    {
        List<GamePiece> horizMatches = FindHorizontalMatches(x, y, 3);
        List<GamePiece> vertMatches = FindVerticalMatches(x, y, 3);

        if (horizMatches == null)
            horizMatches = new List<GamePiece>();

        if (vertMatches == null)
            vertMatches = new List<GamePiece>();

        var combinedMatches = horizMatches.Union(vertMatches).ToList();

        return combinedMatches;
    }

    void ClearPieceAt(int x, int y)
    {
        GamePiece pieceToClear = m_allGamePieces[x, y];

        if (pieceToClear != null)
        {
            m_allGamePieces[x, y] = null;
            Destroy(pieceToClear.gameObject);
        }

        HighlightTileOff(x, y);
    }

    void ClearBoard()
    {
        for (int i = 0; i < width;i++)
        {
            for (int j = 0; j < height; j++)
            {
                ClearPieceAt(i, j);
            }
        }
    }

    void ClearPieceAt(List<GamePiece> gamePieces)
    {
        foreach(var piece in gamePieces)
        {
            ClearPieceAt(piece.xIndex, piece.yIndex);
        }
    }
}
