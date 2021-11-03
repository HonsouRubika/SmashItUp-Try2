using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    private GameManager GM;
    public TextMeshProUGUI timerUI;

    [Space]
    public float durationMiniGame = 30;
    public float miniGameTimer = 0;

    private void Start()
    {
        miniGameTimer = durationMiniGame;

        GM = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void Update()
    {
        if (miniGameTimer <= 0)
        {
            //GM.NextMap();
            GM.Score();
        }
        else
        {
            miniGameTimer -= Time.deltaTime;
            DisplayTimer();
        }
    }

    private void DisplayTimer()
    {
        timerUI.text = UnityEngine.Mathf.Round(miniGameTimer).ToString();
    }
}
