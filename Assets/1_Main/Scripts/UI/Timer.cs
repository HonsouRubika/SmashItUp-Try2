using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public GameObject globalTimer;
    private TextMeshProUGUI timerUI;

    [Space]
    public float durationMiniGame = 30; //not used
    public float miniGameTimer = 0;

    [HideInInspector] public bool isTimerStarted = false;

    public bool debugEndTimer = false;

    public GameObject countdown;
    private GameObject countdownInstance;
    private bool isCountdownSetted = false;


    private Score scoresScript;

    private void Start()
    {
        timerUI = globalTimer.GetComponentInChildren<TextMeshProUGUI>();
        globalTimer.SetActive(false);

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
            Destroy(countdownInstance);
            StopTimer();
            GameManager.Instance.Score();
        }
        else if (isTimerStarted)
        {
            miniGameTimer -= Time.deltaTime;
            DisplayTimer();
        }

        if (miniGameTimer <= 10 && !isCountdownSetted)
        {
            //globalTimer.SetActive(true);
            countdownInstance = Instantiate<GameObject>(countdown);
            isCountdownSetted = true;
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
