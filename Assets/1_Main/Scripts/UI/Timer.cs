using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public TextMeshProUGUI timerUI;

    [Space]
    public float durationMiniGame = 30; //not used
    public float miniGameTimer = 0;

    [HideInInspector] public bool isTimerStarted = false;

    public bool debugEndTimer = false;

    private Score scoresScript;

    private void Start()
    {
        durationMiniGame = GameManager.Instance.durationMiniGame;
        miniGameTimer = durationMiniGame;

        scoresScript = GetComponent<Score>();
    }

    public void Update()
    {
        if (debugEndTimer)
        {
            miniGameTimer = 0;
        }

        if (isTimerStarted && miniGameTimer <= 0)
        {
            //GameManager.Instance.NextMap();
            StopTimer();
            GameManager.Instance.Score();
        }
        else if (isTimerStarted)
        {
            miniGameTimer -= Time.deltaTime;
            DisplayTimer();
        }
    }

    public void StartTimer()
    {
        isTimerStarted = true;
        scoresScript.EnableAddScore();
    }

    public void StopTimer()
    {
        isTimerStarted = false;
        scoresScript.DisableAddScore();
    }

    private void DisplayTimer()
    {
        timerUI.text = UnityEngine.Mathf.Round(miniGameTimer).ToString();
    }
}
