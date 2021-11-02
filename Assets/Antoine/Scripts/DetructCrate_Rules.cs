using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// By Antoine LEROUX
/// This script reference the rules of mini-game Detruct Crate
/// </summary>

public class DetructCrate_Rules : MonoBehaviour
{
    [Header("Players Score")]
    public int scorePlayer0 = 0;
    public int scorePlayer1 = 0;
    public int scorePlayer2 = 0;
    public int scorePlayer3 = 0;
    
    [Header("DetructCreate Rules")]
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
}
