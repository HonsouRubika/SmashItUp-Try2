using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CaptureManager : MonoBehaviour
{
    private GameObject[] playersUnsorted;
    public GameObject[] players;

    public int scorePlayer0 = 0;
    public int scorePlayer1 = 0;
    public int scorePlayer2 = 0;
    public int scorePlayer3 = 0;
    private Zone zoneScript;
    public float timePastInZone = 0;
    public float timeToScore = 1;
    private CaptureSound captureSoundScript;
    public Score scoreScript;
    public Timer timerScript;
    private bool zoneSound = false;

    public List<Transform> tpPoints = new List<Transform>();
    private List<int> randomNumbers = new List<int>();

    private void Start()
    {
        playersUnsorted = GameObject.FindGameObjectsWithTag("Player");
        players = playersUnsorted.OrderBy(go => go.name).ToArray();

        zoneScript = GetComponentInChildren<Zone>();

        captureSoundScript = GetComponentInChildren<CaptureSound>();

        SpawnPlayerRandomly();
        GameManager.Instance.focusPlayersScript.SetGameTitle("Zone");
    }

    private void Update()
    {
        if (timerScript.miniGameTimer <= 0)
        {
            /*int[] scores = new int[4];
            scores[0] = scorePlayer0;
            scores[1] = scorePlayer1;
            scores[2] = scorePlayer2;
            scores[3] = scorePlayer3;
            Array.Sort(scores);*/

            int maxVal = 0;
            int joueurValMax = 0;
            if(scorePlayer0 > maxVal)
            {
                maxVal = scorePlayer0;
                joueurValMax = 0;
            }
            if(scorePlayer1 > maxVal)
            {
                maxVal = scorePlayer1;
                joueurValMax = 1;
            }
            if(scorePlayer2 > maxVal)
            {
                maxVal = scorePlayer2;
                joueurValMax = 2;
            }
            if(scorePlayer3 > maxVal)
            {
                maxVal = scorePlayer3;
                joueurValMax = 3;
            }

            switch (joueurValMax)
            {
                case 0:
                    GameManager.Instance.addScores(10, 0, 0, 0);
                    break;
                case 1:
                    GameManager.Instance.addScores(0, 10, 0, 0);
                    break;
                case 2:
                    GameManager.Instance.addScores(0, 0, 10, 0);
                    break;
                case 3:
                    GameManager.Instance.addScores(0, 0, 0, 10);
                    break;
            }
        }

        if (zoneScript.counterPlayerinZone >= 2)
        {
            timePastInZone = 0;
            captureSoundScript.PlayerOutZone();
            zoneSound = false;

        }
        else
        {
            if (zoneScript.player0IsInZone == true)
            {
                timePastInZone += Time.deltaTime;
                

                if (!zoneSound)
                {
                    zoneSound = true;
                    captureSoundScript.PlayerCapturing();
                }


                if (timePastInZone >= timeToScore)
                {
                    scorePlayer0++;
                    timePastInZone = 0;
                    scoreScript.AddScore(1,0,0,0);
                }
            }          

            if (zoneScript.player1IsInZone == true)
            {
                timePastInZone += Time.deltaTime;             


                if (!zoneSound)
                {
                    zoneSound = true;
                    captureSoundScript.PlayerCapturing();
                }

                if (timePastInZone >= timeToScore)
                {
                    scorePlayer1++;
                    timePastInZone = 0;
                    scoreScript.AddScore(0, 1, 0, 0);
                }
            }

            if (zoneScript.player2IsInZone == true)
            {
                timePastInZone += Time.deltaTime;


                if (!zoneSound)
                {
                    zoneSound = true;
                    captureSoundScript.PlayerCapturing();
                }

                if (timePastInZone >= timeToScore)
                {
                    scorePlayer2++;
                    timePastInZone = 0;
                    scoreScript.AddScore(0, 0, 1, 0);
                }
            }

            if (zoneScript.player3IsInZone == true)
            {
                timePastInZone += Time.deltaTime;


                if (!zoneSound)
                {
                    zoneSound = true;
                    captureSoundScript.PlayerCapturing();
                }

                if (timePastInZone >= timeToScore)
                {
                    scorePlayer3++;
                    timePastInZone = 0;
                    scoreScript.AddScore(0, 0, 0, 1);
                }
            }
        }

        if (zoneScript.counterPlayerinZone < 1)
        {
            timePastInZone = 0;
            captureSoundScript.PlayerOutZone();
            zoneSound = false;
        }
    }

    private void SpawnPlayerRandomly()
    {
        randomNumbers = GenerateRandomNumbers(4, 0, 4);

        for (int i = 0; i < players.Length; i++)
        {
            players[i].transform.position = tpPoints[randomNumbers[i]].position;
        }
    }

    private List<int> GenerateRandomNumbers(int count, int minValue, int maxValue)
    {
        //maxValue is exclusive

        List<int> possibleNumbers = new List<int>();
        List<int> chosenNumbers = new List<int>();

        for (int i = minValue; i < maxValue; i++)
        {
            possibleNumbers.Add(i);
        }

        while (chosenNumbers.Count < count)
        {
            int position = Random.Range(0, possibleNumbers.Count);
            chosenNumbers.Add(possibleNumbers[position]);
            possibleNumbers.RemoveAt(position);
        }
        return chosenNumbers;
    }
}