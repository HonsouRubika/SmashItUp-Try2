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

    [Space]
    //P1
    public Transform P1ScoreNumber;
    private Text P1ScoreUnits;
    private Text P1ScoreTens;
    private Text P1ScoreHundreds;

    //P2
    public Transform P2ScoreNumber;
    private Text P2ScoreUnits;
    private Text P2ScoreTens;
    private Text P2ScoreHundreds;

    //P3
    public Transform P3ScoreNumber;
    private Text P3ScoreUnits;
    private Text P3ScoreTens;
    private Text P3ScoreHundreds;

    //P4
    public Transform P4ScoreNumber;
    private Text P4ScoreUnits;
    private Text P4ScoreTens;
    private Text P4ScoreHundreds;

    public List<GameObject> playerAddedPoints;
    private List<Text> playerAddedPointsText;

    private GameObject[] players;
    public List<Transform> tpPoints = new List<Transform>();

    public GameObject rollingBoard;
    public List<GameObject> scoreBoard;

    private bool isBonus = false;

    private void Start()
    {
        //bonus
        if (GameManager.Instance._nbMancheActu == GameManager.Instance.BonusRound)
        {
            //GameManager.Instance.bonusManagerScript.ApplyBonusInGame();
            isBonus = true;
        }
        else if (GameManager.Instance._nbMancheActu == GameManager.Instance.BonusRound + 1)
        {
            //bonusManagerScript.DisableBonusInGame();
        }

        SpawnPlayer();
        FillPlayerScoreValues();
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
                    scoreBoard[i].SetActive(false);

                    switch (i)
                    {
                        case 0:
                            P1ScoreNumber.gameObject.SetActive(false);
                            break;
                        case 1:
                            P2ScoreNumber.gameObject.SetActive(false);
                            break;
                        case 2:
                            P3ScoreNumber.gameObject.SetActive(false);
                            break;
                        case 3:
                            P4ScoreNumber.gameObject.SetActive(false);
                            break;

                    }
                }    
            }
            rollingBoard.SetActive(true);
        }

        if (Time.time >= timerBeforeUpdateScore && !flagUpdateScore)
        {
            flagUpdateScore = true;

            GameManager.Instance.UpdatePlayerScore();
            DisplayPlayersScore();

            for (int i = 0; i < playerAddedPoints.Count; i++)
            {
                playerAddedPoints[i].SetActive(false);
                scoreBoard[i].SetActive(true);
            }
            P1ScoreNumber.gameObject.SetActive(true);
            P2ScoreNumber.gameObject.SetActive(true);
            P3ScoreNumber.gameObject.SetActive(true);
            P4ScoreNumber.gameObject.SetActive(true);
            rollingBoard.SetActive(false);
        }

        if (Time.time >= TimerNextMapActu)
        {
            //Next Map
            if (GameManager.Instance._nbMancheActu < GameManager.Instance._nbManches && !isBonus)
            {
                GameManager.Instance.NextMap();
                GameManager.Instance.resetScorePoints();
            }
            else if (!isBonus)
            {
                GameManager.Instance.FinaleScore();
                GameManager.Instance.resetScorePoints();
            }
            else
            {
                isBonus = false;

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
            
        }
    }

    private void FillPlayerScoreValues()
    {
        P1ScoreHundreds = P1ScoreNumber.GetChild(0).GetComponent<Text>();
        P1ScoreTens = P1ScoreNumber.GetChild(1).GetComponent<Text>();
        P1ScoreUnits = P1ScoreNumber.GetChild(2).GetComponent<Text>();

        P2ScoreHundreds = P2ScoreNumber.GetChild(0).GetComponent<Text>();
        P2ScoreTens = P2ScoreNumber.GetChild(1).GetComponent<Text>();
        P2ScoreUnits = P2ScoreNumber.GetChild(2).GetComponent<Text>();

        P3ScoreHundreds = P3ScoreNumber.GetChild(0).GetComponent<Text>();
        P3ScoreTens = P3ScoreNumber.GetChild(1).GetComponent<Text>();
        P3ScoreUnits = P3ScoreNumber.GetChild(2).GetComponent<Text>();

        P4ScoreHundreds = P4ScoreNumber.GetChild(0).GetComponent<Text>();
        P4ScoreTens = P4ScoreNumber.GetChild(1).GetComponent<Text>();
        P4ScoreUnits = P4ScoreNumber.GetChild(2).GetComponent<Text>();
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
