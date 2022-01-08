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

    [Header("P1")]
    public Text P1ScoreUnits;
    public Text P1ScoreTens;
    public Text P1ScoreHundreds;

    [Header("P2")]
    public Text P2ScoreUnits;
    public Text P2ScoreTens;
    public Text P2ScoreHundreds;

    [Header("P3")]
    public Text P3ScoreUnits;
    public Text P3ScoreTens;
    public Text P3ScoreHundreds;

    [Header("P4")]
    public Text P4ScoreUnits;
    public Text P4ScoreTens;
    public Text P4ScoreHundreds;

    public List<GameObject> playerAddedPoints;
    private List<Text> playerAddedPointsText;

    private GameObject[] players;
    public List<Transform> tpPoints = new List<Transform>();

    private void Start()
    {
        SpawnPlayer();
        DisplayPlayersScore();

        playerAddedPointsText = new List<Text>(new Text[playerAddedPoints.Count]);
        for (int i = 0; i < playerAddedPoints.Count; i++)
        {
            playerAddedPoints[i].SetActive(false);
            playerAddedPointsText[i] = playerAddedPoints[i].GetComponentInChildren<Text>();
        }

        for (int i = 0; i < playerAddedPointsText.Count; i++)
        {
            if (GameManager.Instance.getAddedPointsPlayer(i + 1) != 0)
            {
                playerAddedPointsText[i].text = "+ " + GameManager.Instance.getAddedPointsPlayer(i + 1);
            }  
        }

        TimerNextMapActu = Time.time + TimerNextMap;
        timerBeforeAddPoints = Time.time + waitBeforeAddPoints;
        timerBeforeUpdateScore = Time.time + waitBeforeUpdateScore;
    }

    private void Update()
    {
        if (Time.time >= timerBeforeAddPoints && !flagAddedPoints)
        {
            flagAddedPoints = true;
            for (int i = 0; i < playerAddedPoints.Count; i++)
            {
                if (GameManager.Instance.getAddedPointsPlayer(i + 1) != 0)
                {
                    playerAddedPoints[i].SetActive(true);
                }    
            }
        }

        if (Time.time >= timerBeforeUpdateScore && !flagUpdateScore)
        {
            flagUpdateScore = true;

            GameManager.Instance.UpdatePlayerScore();
            DisplayPlayersScore();

            for (int i = 0; i < playerAddedPoints.Count; i++)
            {
                playerAddedPoints[i].SetActive(false);
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

    private void DisplayPlayersScore()
    {
        P1ScoreUnits.text = GameManager.Instance.getDividedScorePlayer(1, "units").ToString();
        P1ScoreTens.text = GameManager.Instance.getDividedScorePlayer(1, "tens").ToString();
        P1ScoreHundreds.text = GameManager.Instance.getDividedScorePlayer(1, "hundreds").ToString();

        P2ScoreUnits.text = GameManager.Instance.getDividedScorePlayer(2, "units").ToString();
        P2ScoreTens.text = GameManager.Instance.getDividedScorePlayer(2, "tens").ToString();
        P2ScoreHundreds.text = GameManager.Instance.getDividedScorePlayer(2, "hundreds").ToString();

        P3ScoreUnits.text = GameManager.Instance.getDividedScorePlayer(3, "units").ToString();
        P3ScoreTens.text = GameManager.Instance.getDividedScorePlayer(3, "tens").ToString();
        P3ScoreHundreds.text = GameManager.Instance.getDividedScorePlayer(3, "hundreds").ToString();

        P4ScoreUnits.text = GameManager.Instance.getDividedScorePlayer(4, "units").ToString();
        P4ScoreTens.text = GameManager.Instance.getDividedScorePlayer(4, "tens").ToString();
        P4ScoreHundreds.text = GameManager.Instance.getDividedScorePlayer(4, "hundreds").ToString();
    }

    public void setHardScore(int playerID, int score)
    {
        switch (playerID)
        {
            case 1:
                P1ScoreUnits.text = (score % 10).ToString();
                P1ScoreTens.text = ((score / 10) % 10).ToString();
                P1ScoreHundreds.text = ((score / 100) % 10).ToString();
                GameManager.Instance._scoreP1 = score;
                break;
            case 2:
                P2ScoreUnits.text = (score % 10).ToString();
                P2ScoreTens.text = ((score / 10) % 10).ToString();
                P2ScoreHundreds.text = ((score / 100) % 10).ToString();
                GameManager.Instance._scoreP1 = score;
                break;
            case 3:
                P3ScoreUnits.text = (score % 10).ToString();
                P3ScoreTens.text = ((score / 10) % 10).ToString();
                P3ScoreHundreds.text = ((score / 100) % 10).ToString();
                GameManager.Instance._scoreP1 = score;
                break;
            case 4:
                P4ScoreUnits.text = (score % 10).ToString();
                P4ScoreTens.text = ((score / 10) % 10).ToString();
                P4ScoreHundreds.text = ((score / 100) % 10).ToString();
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
