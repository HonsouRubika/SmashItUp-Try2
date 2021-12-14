using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    [Header("Score Settings")]
    public float TimerNextMap = 3;
    private float TimerNextMapActu = 0;
    public float waitBeforeAddPoints = 0;
    private float timerBeforeAddPoints = 0;
    public float waitBeforeUpdateScore = 0;
    private float timerBeforeUpdateScore = 0;

    private bool flagAddedPoints = false;
    private bool flagUpdateScore = false;

    public List<Text> playerScore;
    public List<Text> playerAddedPoints;

    private GameObject[] players;
    public List<Transform> tpPoints = new List<Transform>();

    private void Start()
    {
        //appel de la fonction setScore du ScoreManager

        for (int i = 0; i < playerScore.Count; i++)
        {
            playerScore[i].text = "Player " + (i + 1) + " : " + GameManager.Instance.getScorePlayer(i + 1);
        }

        for (int i = 0; i < playerAddedPoints.Count; i++)
        {
            playerAddedPoints[i].enabled = false;

            if (GameManager.Instance.getAddedPointsPlayer(i + 1) != 0)
            {
                playerAddedPoints[i].text = "+ " + GameManager.Instance.getAddedPointsPlayer(i + 1);
            }  
        }

        TimerNextMapActu = Time.time + TimerNextMap;
        timerBeforeAddPoints = Time.time + waitBeforeAddPoints;
        timerBeforeUpdateScore = Time.time + waitBeforeUpdateScore;
        SpawnPlayer();
    }

    private void Update()
    {
        if (Time.time >= timerBeforeAddPoints && !flagAddedPoints)
        {
            flagAddedPoints = true;
            for (int i = 0; i < playerAddedPoints.Count; i++)
            {
                playerAddedPoints[i].enabled = true;
            }
        }

        if (Time.time >= timerBeforeUpdateScore && !flagUpdateScore)
        {
            flagUpdateScore = true;

            GameManager.Instance.UpdatePlayerScore();
            for (int i = 0; i < playerScore.Count; i++)
            {
                playerScore[i].text = "Player " + (i + 1) + " : " + GameManager.Instance.getScorePlayer(i + 1);
            }
            for (int i = 0; i < playerAddedPoints.Count; i++)
            {
                playerAddedPoints[i].enabled = false;
            }
        }


        if (Time.time >= TimerNextMapActu)
        {
            //Next Map
            if (GameManager.Instance._nbMancheActu < GameManager.Instance._nbManches)
            {
                GameManager.Instance.NextMap();
                GameManager.Instance.resetScorePoints();
            }
            else
            {
                GameManager.Instance.FinaleScore();
                GameManager.Instance.resetScorePoints();
            }
            
        }
    }

    public void setHardScore(int playerID, int score)
    {
        switch (playerID)
        {
            case 1:
                playerScore[0].text = "Player 1 : " + score;
                GameManager.Instance._scoreP1 = score;
                break;
            case 2:
                playerScore[1].text = "Player 2 : " + score;
                GameManager.Instance._scoreP1 = score;
                break;
            case 3:
                playerScore[2].text = "Player 3 : " + score;
                GameManager.Instance._scoreP1 = score;
                break;
            case 4:
                playerScore[3].text = "Player 4 : " + score;
                GameManager.Instance._scoreP1 = score;
                break;
            default:
                Debug.Log("error UI score");
                break;
        }
    }

    private void SpawnPlayer()
    {
        players = GameObject.FindGameObjectsWithTag("Player");

        for (int i = 0; i < players.Length; i++)
        {
            players[i].transform.position = tpPoints[i].position;
        }
    }
}
