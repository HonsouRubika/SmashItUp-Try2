using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// By Antoine LEROUX
/// This script reference the rules of mini-game Detruct Crate
/// </summary>

public class DetructCrate_Rules : MonoBehaviour
{
    private GameObject[] playersUnsorted;
    public GameObject[] players;

    public int cratesNumber = 0;

    [Space]
    public Score scoreScript;
    public Timer timerScript;

    public List<Transform> tpPoints = new List<Transform>();
    private List<int> randomNumbers = new List<int>();

    private void Start()
    {
        playersUnsorted = GameObject.FindGameObjectsWithTag("Player");
        players = playersUnsorted.OrderBy(go => go.name).ToArray();

        SpawnPlayerRandomly();
        GameManager.Instance.focusPlayersScript.SetGameTitle("DestroyCrates");
    }

    private void Update()
    {
        if (timerScript.miniGameTimer <= 0)
        {
            int maxVal = 0;
            int joueurValMax = 0;
            if (scoreScript.scorePlayer0 > maxVal)
            {
                maxVal = scoreScript.scorePlayer0;
                joueurValMax = 0;
            }
            if (scoreScript.scorePlayer1 > maxVal) 
            {
                maxVal = scoreScript.scorePlayer1;
                joueurValMax = 1;
            }
            if (scoreScript.scorePlayer2 > maxVal)
            {
                maxVal = scoreScript.scorePlayer2;
                joueurValMax = 2;
            }
            if (scoreScript.scorePlayer3 > maxVal)
            {
                maxVal = scoreScript.scorePlayer3;
                joueurValMax = 3;
            }

            switch (joueurValMax)
            {
                case 0:
                    GameManager.Instance.addScores(10, 0, 0, 0);
                    break;
                case 1:
                    GameManager.Instance.addScores(0, 10, 0, 0);
                    break;
                case 2:
                    GameManager.Instance.addScores(0, 0, 10, 0);
                    break;
                case 3:
                    GameManager.Instance.addScores(0, 0, 0, 10);
                    break;
            }
        }

        //End the mini-game
        if (cratesNumber <= 0)
        {
            GameManager.Instance.Score();
        }
    }

    private void SpawnPlayerRandomly()
    {
        randomNumbers = GenerateRandomNumbers(4, 0, 4);

        for (int i = 0; i < players.Length; i++)
        {
            players[i].transform.position = tpPoints[randomNumbers[i]].position;
        }
    }

    private List<int> GenerateRandomNumbers(int count, int minValue, int maxValue)
    {
        //maxValue is exclusive

        List<int> possibleNumbers = new List<int>();
        List<int> chosenNumbers = new List<int>();

        for (int i = minValue; i < maxValue; i++)
        {
            possibleNumbers.Add(i);
        }

        while (chosenNumbers.Count < count)
        {
            int position = Random.Range(0, possibleNumbers.Count);
            chosenNumbers.Add(possibleNumbers[position]);
            possibleNumbers.RemoveAt(position);
        }
        return chosenNumbers;
    }
}
