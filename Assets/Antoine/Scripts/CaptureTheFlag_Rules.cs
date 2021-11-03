using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// By Antoine LEROUX
/// This script reference the rules of mini-game Capture the flag
/// </summary>

public class CaptureTheFlag_Rules : MonoBehaviour
{
    public GameManager GM;

    [Header("CaptureTheFlag Rules")]
    public int winPoints;

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
}
