using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureTheFlag_Rules : MonoBehaviour
{
    /// <summary>
    /// By Antoine LEROUX
    /// This script reference the rules of mini-game Capture the flag
    /// </summary>

    [Header("CaptureTheFlag Rules")]
    public float durationMiniGame = 30;
    public float miniGameTimer = 0;

    public void Update()
    {
        if (miniGameTimer >= durationMiniGame)
        {
            //Call here the function score and next map from MiniGameManager
        }
        else
        {
            miniGameTimer += Time.deltaTime;
        }
    }

    public void FlagCaptured(int playerWin)
    {
        //Call here the function score and next map from MiniGameManager
        Debug.Log("Win");
    }
}
