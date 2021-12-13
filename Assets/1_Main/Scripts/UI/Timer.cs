using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public TextMeshProUGUI timerUI;

    [Space]
    public float durationMiniGame = 30;
    public float miniGameTimer = 0;

    private bool isTimerStarted = false;

    private Score scoresScript;

    private void Start()
    {
        miniGameTimer = durationMiniGame;

        scoresScript = GetComponent<Score>();
    }

    public void Update()
    {
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
        Debug.Log("timer stopped");
        isTimerStarted = false;
        scoresScript.DisableAddScore();
    }

    private void DisplayTimer()
    {
        timerUI.text = UnityEngine.Mathf.Round(miniGameTimer).ToString();
    }
}
