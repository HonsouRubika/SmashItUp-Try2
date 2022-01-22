using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Score : MonoBehaviour
{
    public bool enableDisplayScore = true;
    private bool enableAddScore = false;

    public TextMeshProUGUI score0;
    public TextMeshProUGUI score1;
    public TextMeshProUGUI score2;
    public TextMeshProUGUI score3;

    private GameObject bulleScore0;
    private GameObject bulleScore1;
    private GameObject bulleScore2;
    private GameObject bulleScore3;

    [Header("Players Score")]
    public int scorePlayer0 = 0;
    public int scorePlayer1 = 0;
    public int scorePlayer2 = 0;
    public int scorePlayer3 = 0;

    private void Start()
    {
        bulleScore0 = score0.transform.parent.gameObject;
        bulleScore1 = score1.transform.parent.gameObject;
        bulleScore2 = score2.transform.parent.gameObject;
        bulleScore3 = score3.transform.parent.gameObject;

        DisplayUIDependingPlayers();
    }

    private void DisplayScore()
    {
        score0.text = scorePlayer0.ToString();
        score1.text = scorePlayer1.ToString();
        score2.text = scorePlayer2.ToString();
        score3.text = scorePlayer3.ToString();
    }

    //Use this function to add a score to players, like in destruct crate mini-game
    public void AddScore(int scoreP0, int scoreP1, int scoreP2, int scoreP3)
    {
        if (enableAddScore)
        {
            scorePlayer0 += scoreP0;
            scorePlayer1 += scoreP1;
            scorePlayer2 += scoreP2;
            scorePlayer3 += scoreP3;

            DisplayScore();
        }
    }

    public void SetScore(int scoreP0, int scoreP1, int scoreP2, int scoreP3)
    {
        if (enableAddScore)
        {
            scorePlayer0 = scoreP0;
            scorePlayer1 = scoreP1;
            scorePlayer2 = scoreP2;
            scorePlayer3 = scoreP3;

            DisplayScore();
        }
    }

    //Use this function to set a position to a player, like last place in wolf mini-game
    public void AddPosition(int positionP0, int positionP1, int positionP2, int positionP3)
    {
        if (enableAddScore)
        {
            scorePlayer0 = positionP0;
            scorePlayer1 = positionP1;
            scorePlayer2 = positionP2;
            scorePlayer3 = positionP3;

            DisplayScore();
        }
    }

    public void DisableAddScore()
    {
        enableAddScore = false;
    }

    public void EnableAddScore()
    {
        enableAddScore = true;
    }

    private void DisplayUIDependingPlayers()
    {
        if (enableDisplayScore)
        {
            switch (GameManager.Instance.players.Length)
            {
                case 2:
                    bulleScore0.SetActive(true);
                    bulleScore1.SetActive(true);
                    bulleScore2.SetActive(false);
                    bulleScore3.SetActive(false);
                    break;
                case 3:
                    bulleScore0.SetActive(true);
                    bulleScore1.SetActive(true);
                    bulleScore2.SetActive(true);
                    bulleScore3.SetActive(false);
                    break;
                case 4:
                    bulleScore0.SetActive(true);
                    bulleScore1.SetActive(true);
                    bulleScore2.SetActive(true);
                    bulleScore3.SetActive(true);
                    break;
            }
        }
        else
        {
            bulleScore0.SetActive(false);
            bulleScore1.SetActive(false);
            bulleScore2.SetActive(false);
            bulleScore3.SetActive(false);
        } 
    }
}
