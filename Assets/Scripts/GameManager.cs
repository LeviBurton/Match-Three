using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public int movesLeft = 30;
    public int scoreGoal = 10000;
    public ScreenFader screenFader;
    public TextMeshProUGUI levelNameText;

    Board m_board;
    bool m_isReadyToBegin = false;
    bool m_isGameOver = false;
    bool m_isWinner = false;

    void Start()
    {
        m_board = GameObject.FindObjectOfType<Board>().GetComponent<Board>();
        Scene scene = SceneManager.GetActiveScene();

        if (levelNameText != null)
        {
            levelNameText.text = scene.name;
        }

        StartCoroutine("ExecuteGameLoop");
    }

    IEnumerator ExecuteGameLoop()
    {
        yield return StartCoroutine("StartGameRoutine");
        yield return StartCoroutine("PlayGameRoutine");
        yield return StartCoroutine("EndGameRoutine");
    }

    IEnumerator StartGameRoutine()
    {
        while (!m_isReadyToBegin)
        {
            yield return null;
            yield return new WaitForSeconds(2f);
            m_isReadyToBegin = true;
        }

        if (screenFader != null)
        {
            screenFader.FadeOff();
        }

        yield return new WaitForSeconds(.5f);

        if (m_board != null)
        {
            m_board.SetupBoard();
        }
    }

    IEnumerator PlayGameRoutine()
    {

        while (!m_isGameOver)
        {
            yield return null;
        }
    }

    IEnumerator EndGameRoutine()
    {
        if (m_isWinner)
        {
            Debug.Log("You win");
        }
        else
        {
            Debug.Log("You lose");
        }

        yield return null;
    }
}
