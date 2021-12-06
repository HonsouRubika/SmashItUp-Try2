using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// By Antoine LEROUX
/// This script reference the rules of mini-game Capture the flag
/// </summary>

public class CaptureTheFlag_Rules : MonoBehaviour
{
    private GameObject[] playersUnsorted;
    public GameObject[] players;

    public List<Transform> tpPoints = new List<Transform>();
    private List<int> randomNumbers = new List<int>();

    [Header("CaptureTheFlag Rules")]
    public int winPoints;

    private void Start()
    {
        playersUnsorted = GameObject.FindGameObjectsWithTag("Player");
        players = playersUnsorted.OrderBy(go => go.name).ToArray();
        
        SpawnPlayerRandomly();
        GameManager.Instance.focusPlayersScript.SetGameTitle("CaptureTheFlag");
    }

    public void FlagCaptured(int playerWin)
    {
        switch (playerWin)
        {
            case 0:
                GameManager.Instance.addScores(winPoints, 0, 0, 0);
                break;
            case 1:
                GameManager.Instance.addScores(0, winPoints, 0, 0);
                break;
            case 2:
                GameManager.Instance.addScores(0, 0, winPoints, 0);
                break;
            case 3:
                GameManager.Instance.addScores(0, 0, 0, winPoints);
                break;
        }

        //GameManager.Instance.NextMap();
        GameManager.Instance.Score();
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
