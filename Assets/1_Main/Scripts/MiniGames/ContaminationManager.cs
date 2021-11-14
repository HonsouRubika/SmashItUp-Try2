using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ContaminationManager : MonoBehaviour
{
    private GameObject[] playersUnsorted;
    public GameObject[] players;
    private PlayerController[] playersControllers;

    [Header("Who is Wolf ?")]
    public bool player0IsWolf = false;
    public int player0ContaminationOrder = 0;
    public bool player1IsWolf = false;
    public int player1ContaminationOrder = 0;
    public bool player2IsWolf = false;
    public int player2ContaminationOrder = 0;
    public bool player3IsWolf = false;
    public int player3ContaminationOrder = 0;

    private int contaminationOrder = 0;
    private int wolfPlayerNumber;

    [Header("Wolf visual")]
    public GameObject wolfHeadPrefab;
    public float positionYHead;
    private GameObject[] wolfHeadInstances;

    [Header("UI")]
    public Score scoreScript;
    public Timer timerScript;

    private void Start()
    {
        playersUnsorted = GameObject.FindGameObjectsWithTag("Player");
        players = playersUnsorted.OrderBy(go => go.name).ToArray();

        wolfHeadInstances = new GameObject[players.Length];

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


        if (timerScript.miniGameTimer <= 0 || contaminationOrder == players.Length)
        {
            //DestroyWolfHead();
            SortPlayers(1, player0ContaminationOrder);
            SortPlayers(2, player1ContaminationOrder);
            SortPlayers(3, player2ContaminationOrder);
            SortPlayers(4, player3ContaminationOrder);
        }      
    }

    private void NewPlayerIsWolf(int playerNumber)
    {
        contaminationOrder++;

        switch (playerNumber)
        {
            case 0:
                player0IsWolf = true;
                player0ContaminationOrder = contaminationOrder;
                break;
            case 1:
                player1IsWolf = true;
                player1ContaminationOrder = contaminationOrder;
                break;
            case 2:
                player2IsWolf = true;
                player2ContaminationOrder = contaminationOrder;
                break;
            case 3:
                player3IsWolf = true;
                player3ContaminationOrder = contaminationOrder;
                break;
        }
    }

    private void SpawnWolfHead(int playerNumber)
    {
        wolfHeadInstances[playerNumber] = Instantiate(wolfHeadPrefab, new Vector2(players[playerNumber].transform.position.x, players[playerNumber].transform.position.y + positionYHead), Quaternion.identity);
        wolfHeadInstances[playerNumber].transform.SetParent(players[playerNumber].transform);
    }

    private void DestroyWolfHead()
    {
        for (int i = 0; i < wolfHeadInstances.Length; i++)
        {
            Destroy(wolfHeadInstances[i]);
        }
    }

    private void SortPlayers(int player, int playerOrder)
    {
        if (contaminationOrder == players.Length)   //all players have been contamined
        {
            switch (playerOrder)
            {
                case 0: //not contamined

                    break;
                case 1: //original wolf
                    GameManager.Instance.addSpecificScore(player, 10);
                    break;
                case 2: //first contamined
                    GameManager.Instance.addSpecificScore(player, 8);
                    break;
                case 3: //second contamined
                    GameManager.Instance.addSpecificScore(player, 6);
                    break;
                case 4: //third contamined
                    GameManager.Instance.addSpecificScore(player, 4);
                    break;
            }
        }
        else  //remain players not contamined
        {
            switch (playerOrder)
            {
                case 0: //not contamined
                    GameManager.Instance.addSpecificScore(player, 10);
                    break;
                case 1: //original wolf
                    GameManager.Instance.addSpecificScore(player, 4);
                    break;
                case 2: //first contamined
                    GameManager.Instance.addSpecificScore(player, 6);
                    break;
                case 3: //second contamined
                    GameManager.Instance.addSpecificScore(player, 8);
                    break;
                case 4: //third contamined

                    break;
            }
        }     
    }
}
