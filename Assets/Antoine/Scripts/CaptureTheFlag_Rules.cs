using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// By Antoine LEROUX
/// This script reference the rules of mini-game Capture the flag
/// </summary>

public class CaptureTheFlag_Rules : MonoBehaviour
{
    private GameManager GM;

    [Header("TP Points")]
    public Transform tpPoints0;
    public Transform tpPoints1;
    public Transform tpPoints2;
    public Transform tpPoints3;

    [Header("CaptureTheFlag Rules")]
    public int winPoints;

    private void Start()
    {
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

    public void FlagCaptured(int playerWin)
    {
        switch (playerWin)
        {
            case 0:
                GM.addScores(winPoints, 0, 0, 0);
                break;
            case 1:
                GM.addScores(0, winPoints, 0, 0);
                break;
            case 2:
                GM.addScores(0, 0, winPoints, 0);
                break;
            case 3:
                GM.addScores(0, 0, 0, winPoints);
                break;
        }

        //GM.NextMap();
        GM.Score();
    }
}
