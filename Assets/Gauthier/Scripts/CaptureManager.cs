using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CaptureManager : MonoBehaviour
{
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

    private GameManager GM;

    [Header("TP Points")]
    public Transform tpPoints0;
    public Transform tpPoints1;
    public Transform tpPoints2;
    public Transform tpPoints3;

    private void Start()
    {
        zoneScript = GetComponentInChildren<Zone>();

        captureSoundScript = GetComponentInChildren<CaptureSound>();

        GM = GameObject.Find("GameManager").GetComponent<GameManager>();

        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            switch (player.GetComponent<PlayerController>().playerID)
            {
                case 0:
                    player.transform.position = tpPoints0.position;
                    break;
                case 1:
                    player.transform.position = tpPoints1.position;
                    break;
                case 2:
                    player.transform.position = tpPoints2.position;
                    break;
                case 3:
                    player.transform.position = tpPoints3.position;
                    break;
            }
        }
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
                    GM.addScores(10, 0, 0, 0);
                    break;
                case 1:
                    GM.addScores(0, 10, 0, 0);
                    break;
                case 2:
                    GM.addScores(0, 0, 10, 0);
                    break;
                case 3:
                    GM.addScores(0, 0, 0, 10);
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
}