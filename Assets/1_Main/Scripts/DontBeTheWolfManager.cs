using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontBeTheWolfManager : MonoBehaviour
{
    public GameObject[] players;
    private PlayerController[] playersControllers;

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
        players = GameObject.FindGameObjectsWithTag("Player");

        playersControllers = new PlayerController[players.Length];
        for (int i = 0; i < players.Length; i++)
        {
            playersControllers[i] = players[i].GetComponent<PlayerController>();
        }

        ChooseRandomWolf();
    }

    private void ChooseRandomWolf()
    {
        wolfPlayerNumber = Random.Range(0, players.Length);

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

        SortPlayers();
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
        if (timerScript.miniGameTimer <= 0)
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
    }
}
