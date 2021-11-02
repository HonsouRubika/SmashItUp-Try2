using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// By Antoine LEROUX
/// This script reference the rules of mini-game Detruct Crate
/// </summary>

public class DetructCrate_Rules : MonoBehaviour
{
    public TextMeshProUGUI timerUI;
    public TextMeshProUGUI score0;
    public TextMeshProUGUI score1;
    public TextMeshProUGUI score2;
    public TextMeshProUGUI score3;

    [Header("Players Score")]
    public int scorePlayer0 = 0;
    public int scorePlayer1 = 0;
    public int scorePlayer2 = 0;
    public int scorePlayer3 = 0;
    
    [Header("DetructCreate Rules")]
    public float durationMiniGame = 30;
    public float miniGameTimer = 0;

    private void Start()
    {
        miniGameTimer = durationMiniGame;
    }

    public void Update()
    {
        if (miniGameTimer <= 0)
        {
            //GM.NextMap();
        }
        else
        {
            miniGameTimer -= Time.deltaTime;
            DisplayTimer();
            DisplayScore();
        }
    }

    private void DisplayTimer()
    {
        timerUI.text = UnityEngine.Mathf.Round(miniGameTimer).ToString();
    }

    private void DisplayScore()
    {
        score0.text = scorePlayer0.ToString();
        score1.text = scorePlayer1.ToString();
        score2.text = scorePlayer2.ToString();
        score3.text = scorePlayer3.ToString();
    }
}
