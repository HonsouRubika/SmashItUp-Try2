using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DontBeTheWolfManager : MonoBehaviour
{
    private GameObject[] playersUnsorted;
    public GameObject[] players;
    private List<GameObject> playersNotWolf;
    private PlayerController[] playersControllers;

    public Transform wolfTpPoint;
    public List<Transform> tpPoints = new List<Transform>();
    private List<int> randomNumbers = new List<int>();

    [Header("Who is Wolf ?")]
    public bool player0IsWolf = false;
    public float scorePlayer0 = 0;
    public bool player1IsWolf = false;
    public float scorePlayer1 = 0;
    public bool player2IsWolf = false;
    public float scorePlayer2 = 0;
    public bool player3IsWolf = false;
    public float scorePlayer3 = 0;

    private int wolfPlayerNumber;

    [Header("Wolf visual")]
    public GameObject wolfHeadPrefab;
    public float positionYHead;
    private GameObject wolfHeadInstance;

    [Header("UI")]
    public Score scoreScript;
    public Timer timerScript;

    private void Start()
    {
        playersUnsorted = GameObject.FindGameObjectsWithTag("Player");
        players = playersUnsorted.OrderBy(go => go.name).ToArray();
        playersNotWolf = players.ToList();

        playersControllers = new PlayerController[players.Length];
        for (int i = 0; i < players.Length; i++)
        {
            playersControllers[i] = players[i].GetComponent<PlayerController>();
        }

        ChooseRandomWolf();
        SpawnPlayerRandomly();
    }

    private void ChooseRandomWolf()
    {
        wolfPlayerNumber = Random.Range(0, players.Length);

        playersNotWolf.Remove(players[wolfPlayerNumber]);

        NewPlayerIsWolf(wolfPlayerNumber);

        SpawnWolfHead(wolfPlayerNumber);
    }

    private void Update()
    {
        if (playersControllers[wolfPlayerNumber].hitPlayer)
        {
            wolfPlayerNumber = playersControllers[wolfPlayerNumber].playerIDHit;

            NewPlayerIsWolf(wolfPlayerNumber);
            SpawnWolfHead(wolfPlayerNumber);
        }

        IncrementPlayerScore(wolfPlayerNumber);

        if (timerScript.miniGameTimer <= 0)
        {
            SortPlayers();
        }
    }

    private void NewPlayerIsWolf(int playerNumber)
    {
        switch (playerNumber)
        {
            case 0:
                player0IsWolf = true;
                player1IsWolf = false;
                player2IsWolf = false;
                player3IsWolf = false;
                break;
            case 1:
                player0IsWolf = false;
                player1IsWolf = true;
                player2IsWolf = false;
                player3IsWolf = false;
                break;
            case 2:
                player0IsWolf = false;
                player1IsWolf = false;
                player2IsWolf = true;
                player3IsWolf = false;
                break;
            case 3:
                player0IsWolf = false;
                player1IsWolf = false;
                player2IsWolf = false;
                player3IsWolf = true;
                break;
        }
    }

    private void SpawnWolfHead(int playerNumber)
    {
        if (wolfHeadInstance != null)
        {
            Destroy(wolfHeadInstance);
        }
        wolfHeadInstance = Instantiate(wolfHeadPrefab, new Vector2(players[playerNumber].transform.position.x, players[playerNumber].transform.position.y + positionYHead), Quaternion.identity);
        wolfHeadInstance.transform.SetParent(players[playerNumber].transform);
    }

    private void IncrementPlayerScore(int playerNumber)
    {
        scoreScript.SetScore((int)scorePlayer0, (int)scorePlayer1, (int)scorePlayer2, (int)scorePlayer3);

        switch (playerNumber)
        {
            case 0:
                scorePlayer0 += Time.deltaTime;
                break;
            case 1:
                scorePlayer1 += Time.deltaTime;
                break;
            case 2:
                scorePlayer2 += Time.deltaTime;
                break;
            case 3:
                scorePlayer3 += Time.deltaTime;
                break;
        }
    }

    private void SortPlayers()
    {
        float maxVal = 0;
        int joueurValMax = 0;
        if (scorePlayer0 > maxVal)
        {
            maxVal = scorePlayer0;
            joueurValMax = 0;
        }
        if (scorePlayer1 > maxVal)
        {
            maxVal = scorePlayer1;
            joueurValMax = 1;
        }
        if (scorePlayer2 > maxVal)
        {
            maxVal = scorePlayer2;
            joueurValMax = 2;
        }
        if (scorePlayer3 > maxVal)
        {
            maxVal = scorePlayer3;
            joueurValMax = 3;
        }

        switch (joueurValMax)
        {
            case 0:
                GameManager.Instance.addScores(0, 10, 10, 10);
                break;
            case 1:
                GameManager.Instance.addScores(10, 0, 10, 10);
                break;
            case 2:
                GameManager.Instance.addScores(10, 10, 0, 10);
                break;
            case 3:
                GameManager.Instance.addScores(10, 10, 10, 0);
                break;
        }
    }

    private void SpawnPlayerRandomly()
    {
        randomNumbers = GenerateRandomNumbers(3, 0, 3);

        for (int i = 0; i < playersNotWolf.Count; i++)
        {
            playersNotWolf[i].transform.position = tpPoints[randomNumbers[i]].position;
        }

        players[wolfPlayerNumber].transform.position = wolfTpPoint.position;
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
