using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// By Antoine LEROUX
/// This script reference the rules of mini-game Capture the flag
/// </summary>

public class CaptureTheFlag_Rules : MonoBehaviour
{
    public GameManager GM;
    public TextMeshProUGUI timerUI;

    [Header("CaptureTheFlag Rules")]
    public float durationMiniGame = 30;
    public float miniGameTimer = 0;
    public int winPoints;

    private void Start()
    {
        miniGameTimer = durationMiniGame;
    }

    public void Update()
    {
        if (miniGameTimer <= 0)
        {
            GM.NextMap();
        }
        else
        {
            miniGameTimer -= Time.deltaTime;
            DisplayTimer();
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

        GM.NextMap();
    }

    private void DisplayTimer()
    {
        timerUI.text = UnityEngine.Mathf.Round(miniGameTimer).ToString();
    }
}
