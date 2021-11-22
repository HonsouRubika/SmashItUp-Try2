using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class KeepTheFlagManager : MonoBehaviour
{
    private GameObject[] playersUnsorted;
    public GameObject[] players;
    private PlayerController[] playersControllers;

    private KeepingFlag keepingFlagScript;

    [Header("UI")]
    public Score scoreScript;
    public Timer timerScript;

    public List<Transform> tpPoints = new List<Transform>();
    private List<int> randomNumbers = new List<int>();

    [Header("KeepFlag Rules")]
    public int winPoints;

    [Header("KeepFlag Score")]
    public bool player0HaveFlag = false;
    public bool player1HaveFlag = false;
    public bool player2HaveFlag = false;
    public bool player3HaveFlag = false;
    public float[] scorePlayers;
    private float[] finalScores;
    private int[] playersPosition;

    public int currentPlayerHaveFlag = 0;

    private void Start()
    {
        keepingFlagScript = GetComponentInChildren<KeepingFlag>();

        playersUnsorted = GameObject.FindGameObjectsWithTag("Player");
        players = playersUnsorted.OrderBy(go => go.name).ToArray();

        playersControllers = new PlayerController[players.Length];
        scorePlayers = new float[players.Length];
        for (int i = 0; i < players.Length; i++)
        {
            playersControllers[i] = players[i].GetComponent<PlayerController>();
        }

        finalScores = new float[players.Length];
        playersPosition = new int[players.Length];
        for (int i = 0; i < players.Length; i++)
        {
            playersPosition[i] = i;
        }

        SpawnPlayerRandomly();
    }

    private void Update()
    {
        IncrementPlayerScore();

        for (int i = 0; i < playersControllers.Length; i++)
        {
            if (playersControllers[i].hitPlayer)
            {
                if (playersControllers[i].playerIDHit == currentPlayerHaveFlag - 1)     //Attacked the player who have the flag
                {
                    FlagCaptured((int)playersControllers[i].playerID);
                    keepingFlagScript.AttachFlagToPlayer(players[(int)playersControllers[i].playerID].transform);
                }
            }
        }

        if (timerScript.miniGameTimer <= 0)
        {
            SortPlayers();
        }
    }

    public void FlagCaptured(int player)
    {
        switch (player)
        {
            case 0:
                player0HaveFlag = true;
                player1HaveFlag = false;
                player2HaveFlag = false;
                player3HaveFlag = false;
                break;
            case 1:
                player0HaveFlag = false;
                player1HaveFlag = true;
                player2HaveFlag = false;
                player3HaveFlag = false;
                break;
            case 2:
                player0HaveFlag = false;
                player1HaveFlag = false;
                player2HaveFlag = true;
                player3HaveFlag = false;
                break;
            case 3:
                player0HaveFlag = false;
                player1HaveFlag = false;
                player2HaveFlag = false;
                player3HaveFlag = true;
                break;
        }
    }

    private void IncrementPlayerScore()
    {
        if (scorePlayers.Length == 2)
        {
            scoreScript.SetScore((int)scorePlayers[0], (int)scorePlayers[1], 0, 0);
        }
        else if (scorePlayers.Length == 3)
        {
            scoreScript.SetScore((int)scorePlayers[0], (int)scorePlayers[1], (int)scorePlayers[2], 0);
        }
        else if (scorePlayers.Length == 4)
        {
            scoreScript.SetScore((int)scorePlayers[0], (int)scorePlayers[1], (int)scorePlayers[2], (int)scorePlayers[3]);
        }

        if (player0HaveFlag)
        {
            scorePlayers[0] += Time.deltaTime;
            currentPlayerHaveFlag = 1;
        }
        else if (player1HaveFlag)
        {
            scorePlayers[1] += Time.deltaTime;
            currentPlayerHaveFlag = 2;
        }
        else if (player2HaveFlag)
        {
            scorePlayers[2] += Time.deltaTime;
            currentPlayerHaveFlag = 3;
        }
        else if (player3HaveFlag)
        {
            scorePlayers[3] += Time.deltaTime;
            currentPlayerHaveFlag = 4;
        }
    }

    private void SortPlayers()
    {
        for (int i = 0; i < finalScores.Length; i++)
        {
            finalScores[i] = scorePlayers[i];
        }

        System.Array.Sort(finalScores, playersPosition);

        switch (playersPosition.Length)
        {
            case 2:
                GameManager.Instance.addSpecificScore(playersPosition[playersPosition.Length - 1] + 1, 10);
                GameManager.Instance.addSpecificScore(playersPosition[playersPosition.Length - 2] + 1, 5);
                break;
            case 3:
                GameManager.Instance.addSpecificScore(playersPosition[playersPosition.Length - 1] + 1, 10);
                GameManager.Instance.addSpecificScore(playersPosition[playersPosition.Length - 2] + 1, 6);
                GameManager.Instance.addSpecificScore(playersPosition[playersPosition.Length - 3] + 1, 3);
                break;
            case 4:
                GameManager.Instance.addSpecificScore(playersPosition[playersPosition.Length - 1] + 1, 10);
                GameManager.Instance.addSpecificScore(playersPosition[playersPosition.Length - 2] + 1, 8);
                GameManager.Instance.addSpecificScore(playersPosition[playersPosition.Length - 3] + 1, 6);
                GameManager.Instance.addSpecificScore(playersPosition[playersPosition.Length - 4] + 1, 4);
                break;
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
