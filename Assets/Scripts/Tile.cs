using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType
{
    Normal,
    Obstacle
}

public class Tile : MonoBehaviour
{
    public int xIndex;
    public int yIndex;
    public TileType tileType = TileType.Normal;

    Board m_board;

    public void Init(int x, int y, Board board)
    {
        xIndex = x;
        yIndex = y;
        m_board = board;
    }

    void OnMouseEnter()
    {
        if (m_board == null)
            return;

        m_board.DragToTile(this);
    }

    void OnMouseDown()
    {
        if (m_board == null)
            return;

        m_board.ClickTile(this);
    }

    void OnMouseUp()
    {
        if (m_board == null)
            return;

        m_board.ReleaseTile();
    }
}
