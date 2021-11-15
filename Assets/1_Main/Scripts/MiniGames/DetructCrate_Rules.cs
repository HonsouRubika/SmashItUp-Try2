using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// By Antoine LEROUX
/// This script reference the rules of mini-game Detruct Crate
/// </summary>

public class DetructCrate_Rules : MonoBehaviour
{
    public int cratesNumber = 0;

    [Space]
    public Score scoreScript;
    public Timer timerScript;

    [Header("TP Points")]
    public Transform tpPoints0;
    public Transform tpPoints1;
    public Transform tpPoints2;
    public Transform tpPoints3;

    private void Start()
    {
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
            int maxVal = 0;
            int joueurValMax = 0;
            if (scoreScript.scorePlayer0 > maxVal)
            {
                maxVal = scoreScript.scorePlayer0;
                joueurValMax = 0;
            }
            if (scoreScript.scorePlayer1 > maxVal) 
            {
                maxVal = scoreScript.scorePlayer1;
                joueurValMax = 1;
            }
            if (scoreScript.scorePlayer2 > maxVal)
            {
                maxVal = scoreScript.scorePlayer2;
                joueurValMax = 2;
            }
            if (scoreScript.scorePlayer3 > maxVal)
            {
                maxVal = scoreScript.scorePlayer3;
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

        //End the mini-game
        if (cratesNumber <= 0)
        {
            GameManager.Instance.Score();
        }
    }
}
