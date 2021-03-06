﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MatchValue
{
    None,
    Yellow,
    Blue,
    Magenta,
    Indigo,
    Green,
    Teal,
    Red,
    Cyan,
    Wild,
    Orange
}

public class GamePiece : MonoBehaviour
{
    public int xIndex;
    public int yIndex;

    Board m_board;

    bool m_isMoving = false;
    public InterpType interpolation = InterpType.SmootherStep;
    public MatchValue matchValue;
    public int scoreValue = 20;


    public enum InterpType
    {
        Linear,
        EaseOut,
        EaseIn,
        SmoothStep,
        SmootherStep
    }

    public void Init(Board board)
    {
        m_board = board;
    }

    public void SetCoord(int x, int y)
    {
        xIndex = x;
        yIndex = y;
    }

    public void Move(int destX, int destY, float timeToMove)
    {
        if (!m_isMoving)
        {
            StartCoroutine(MoveRoutine(new Vector3(destX, destY, 0), timeToMove));
        }
    }

    IEnumerator MoveRoutine(Vector3 destination, float timeToMove)
    {
        Vector3 startPosition = transform.position;
        bool reachedDestination = false;

        float elapsedTime = 0f;

        m_isMoving = true;

        while (!reachedDestination)
        {
            // if we have reached our destination
            if (Vector3.Distance(transform.position, destination) < 0.01f)
            {
                reachedDestination = true;
                m_board.PlaceGamePiece(this, (int)destination.x, (int)destination.y);

                break;
            }

            elapsedTime += Time.deltaTime;

            float t = Mathf.Clamp(elapsedTime / timeToMove, 0f, 1f);

            // This is pretty cool stuff here.  Very general and I can re-use this type 
            // of functionality in a lot of places.
            switch (interpolation)
            {
                case InterpType.Linear:
                    break;

                case InterpType.EaseIn:
                    t = 1 - Mathf.Cos(t * Mathf.PI * 0.5f);
                    break;

                case InterpType.EaseOut:
                    t = Mathf.Sin(t * Mathf.PI * 0.5f);
                    break;

                // generates a sigmoid like s-shape ease-in/out curve.
                case InterpType.SmoothStep:
                    t = t * t * (3 - 2 * t);
                    break;

                // generates a sigmoid like s-shape ease-in/out curve.
                case InterpType.SmootherStep:
                    t = t * t * t * (t * (t * 6 - 15) + 10);
                    break;
            }
           
            transform.position = Vector3.Lerp(startPosition, destination, t);

            // wait until next frame
            yield return null;
        }

        m_isMoving = false;
    }

    public void ChangeColor(GamePiece pieceToMatch)
    {
        var rendererToChange = GetComponent<SpriteRenderer>();
        Color colorToMatch = Color.clear;

        if (pieceToMatch != null)
        {
            var rendererToMatch = pieceToMatch.GetComponent<SpriteRenderer>();

            if (rendererToMatch != null && rendererToChange != null)
            {
                rendererToChange.color = rendererToMatch.color;
            }

            matchValue = pieceToMatch.matchValue;
        }
    }

    public void ScorePoints(int multiplier = 1, int bonus = 0)
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(scoreValue * multiplier + bonus);
        }
    }
}
