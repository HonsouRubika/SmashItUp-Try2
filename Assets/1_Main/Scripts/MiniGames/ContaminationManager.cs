using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ContaminationManager : MonoBehaviour
{
    private GameObject[] playersUnsorted;
    public GameObject[] players;
    private List<GameObject> playersNotWolf;
    private PlayerController[] playersControllers;
    public List<int> playersTransformedWolf = new List<int>();
    private WolfSound WolfSoundScript;

    //public Transform wolfTpPoint;
    public List<Transform> tpPoints = new List<Transform>();
    private List<int> randomNumbers = new List<int>();

    [Header("Who is Wolf ?")]
    public bool player0IsWolf = false;
    public int player0ContaminationOrder = 0;
    public bool player1IsWolf = false;
    public int player1ContaminationOrder = 0;
    public bool player2IsWolf = false;
    public int player2ContaminationOrder = 0;
    public bool player3IsWolf = false;
    public int player3ContaminationOrder = 0;

    public int contaminationOrder = 0;
    private int wolfPlayerNumber;

    [Header("Wolf visual")]
    public GameObject wolfHeadPrefab;
    public float positionYHead;
    private GameObject[] wolfHeadInstances;

    [Header("UI")]
    public Score scoreScript;
    public Timer timerScript;

    private bool playOneTime = false;

    private void Start()
    {
        playersUnsorted = GameObject.FindGameObjectsWithTag("Player");
        players = playersUnsorted.OrderBy(go => go.name).ToArray();
        playersNotWolf = players.ToList();
        WolfSoundScript = GetComponentInChildren<WolfSound>();

        wolfHeadInstances = new GameObject[players.Length];

        playersControllers = new PlayerController[players.Length];
        for (int i = 0; i < players.Length; i++)
        {
            playersControllers[i] = players[i].GetComponent<PlayerController>();
        }

        ChooseRandomWolf();
        SpawnPlayerRandomly();
        //GameManager.Instance.focusPlayersScript.SetGameTitle("Contamination");
    }

    private void ChooseRandomWolf()
    {
        wolfPlayerNumber = Random.Range(0, players.Length);

        playersNotWolf.Remove(players[wolfPlayerNumber]);

        NewPlayerIsWolf(wolfPlayerNumber);

        SpawnWolfHead(wolfPlayerNumber);
        WolfSoundScript.WolfScream();
    }

    private void Update()
    {
        for (int i = 0; i < playersTransformedWolf.Count; i++)
        {
            if (playersControllers[playersTransformedWolf[i]].hitPlayer)
            {
                if (!playersTransformedWolf.Contains(playersControllers[playersTransformedWolf[i]].playerIDHit))
                {
                    wolfPlayerNumber = playersControllers[playersTransformedWolf[i]].playerIDHit;

                    NewPlayerIsWolf(wolfPlayerNumber);
                    SpawnWolfHead(wolfPlayerNumber);
                    WolfSoundScript.WolfAttack();
                }
            }
        }

        /*if (playersControllers[wolfPlayerNumber].hitPlayer)
        {
            wolfPlayerNumber = playersControllers[wolfPlayerNumber].playerIDHit;

            NewPlayerIsWolf(wolfPlayerNumber);
            SpawnWolfHead(wolfPlayerNumber);
            WolfSoundScript.WolfAttack();
        }*/

        if ((timerScript.miniGameTimer <= 0 || contaminationOrder == players.Length) && !playOneTime)
        {
            //DestroyWolfHead();

            DestroyWolfHead();
            GameManager.Instance.Score();

            switch (players.Length)
            {
                case 2:
                    SortPlayers(1, player0ContaminationOrder);
                    SortPlayers(2, player1ContaminationOrder);
                    break;
                case 3:
                    SortPlayers(1, player0ContaminationOrder);
                    SortPlayers(2, player1ContaminationOrder);
                    SortPlayers(3, player2ContaminationOrder);
                    break;
                case 4:
                    SortPlayers(1, player0ContaminationOrder);
                    SortPlayers(2, player1ContaminationOrder);
                    SortPlayers(3, player2ContaminationOrder);
                    SortPlayers(4, player3ContaminationOrder);
                    break;
            }

            playOneTime = true;
        }      
    }

    private void NewPlayerIsWolf(int playerNumber)
    {
        playersTransformedWolf.Add(playerNumber);
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
        switch (players.Length)
        {
            case 2:
                if (contaminationOrder == players.Length)   //all players have been contamined
                {
                    switch (playerOrder)
                    {
                        case 1: //original wolf
                            GameManager.Instance.addSpecificScorePoints(player, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                            break;
                        case 2: //first contamined
                            GameManager.Instance.addSpecificScorePoints(player, GameManager.Instance.scoreValuesManagerScript.PointsSecondPlace);
                            break;
                    }
                }
                else  //remain players not contamined
                {
                    switch (playerOrder)
                    {
                        case 0: //not contamined
                            GameManager.Instance.addSpecificScorePoints(player, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                            break;
                        case 1: //original wolf
                            GameManager.Instance.addSpecificScorePoints(player, GameManager.Instance.scoreValuesManagerScript.PointsSecondPlace);
                            break;
                    }
                }
                break;
            case 3:
                if (contaminationOrder == players.Length)   //all players have been contamined
                {
                    switch (playerOrder)
                    {
                        case 1: //original wolf
                            GameManager.Instance.addSpecificScorePoints(player, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                            break;
                        case 2: //first contamined
                            GameManager.Instance.addSpecificScorePoints(player, GameManager.Instance.scoreValuesManagerScript.PointsThirdPlace);
                            break;
                        case 3: //second contamined
                            GameManager.Instance.addSpecificScorePoints(player, GameManager.Instance.scoreValuesManagerScript.PointsSecondPlace);
                            break;
                    }
                }
                else if (contaminationOrder == players.Length - 1) //remain 1 player not contamined
                {
                    switch (playerOrder)
                    {
                        case 0: //not contamined
                            GameManager.Instance.addSpecificScorePoints(player, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                            break;
                        case 1: //original wolf
                            GameManager.Instance.addSpecificScorePoints(player, GameManager.Instance.scoreValuesManagerScript.PointsThirdPlace);
                            break;
                        case 2: //first contamined
                            GameManager.Instance.addSpecificScorePoints(player, GameManager.Instance.scoreValuesManagerScript.PointsSecondPlace);
                            break;
                    }
                }
                else  //players not contamined
                {
                    switch (playerOrder)
                    {
                        case 0: //not contamined
                            GameManager.Instance.addSpecificScorePoints(player, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                            break;
                        case 1: //original wolf
                            GameManager.Instance.addSpecificScorePoints(player, GameManager.Instance.scoreValuesManagerScript.PointsSecondPlace);
                            break;
                    }
                }
                break;
            case 4:
                if (contaminationOrder == players.Length)   //all players have been contamined
                {
                    switch (playerOrder)
                    {
                        case 1: //original wolf
                            GameManager.Instance.addSpecificScorePoints(player, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                            break;
                        case 2: //first contamined
                            GameManager.Instance.addSpecificScorePoints(player, GameManager.Instance.scoreValuesManagerScript.PointsSecondPlace);
                            break;
                        case 3: //second contamined
                            GameManager.Instance.addSpecificScorePoints(player, GameManager.Instance.scoreValuesManagerScript.PointsThirdPlace);
                            break;
                        case 4: //third contamined
                            GameManager.Instance.addSpecificScorePoints(player, GameManager.Instance.scoreValuesManagerScript.PointsFourthPlace);
                            break;
                    }
                }
                else if (contaminationOrder == players.Length - 1) //remain 1 player not contamined
                {
                    switch (playerOrder)
                    {
                        case 0: //not contamined
                            GameManager.Instance.addSpecificScorePoints(player, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                            break;
                        case 1: //original wolf
                            GameManager.Instance.addSpecificScorePoints(player, GameManager.Instance.scoreValuesManagerScript.PointsSecondPlace);
                            break;
                        case 2: //first contamined
                            GameManager.Instance.addSpecificScorePoints(player, GameManager.Instance.scoreValuesManagerScript.PointsThirdPlace);
                            break;
                        case 3: //second contamined
                            GameManager.Instance.addSpecificScorePoints(player, GameManager.Instance.scoreValuesManagerScript.PointsFourthPlace);
                            break;
                    }
                }
                else if (contaminationOrder == players.Length - 2) //remain 2 players not contamined
                {
                    switch (playerOrder)
                    {
                        case 0: //not contamined
                            GameManager.Instance.addSpecificScorePoints(player, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                            break;
                        case 1: //original wolf
                            GameManager.Instance.addSpecificScorePoints(player, GameManager.Instance.scoreValuesManagerScript.PointsSecondPlace);
                            break;
                        case 2: //first contamined
                            GameManager.Instance.addSpecificScorePoints(player, GameManager.Instance.scoreValuesManagerScript.PointsThirdPlace);
                            break;
                    }
                }
                else  //players not contamined
                {
                    switch (playerOrder)
                    {
                        case 0: //not contamined
                            GameManager.Instance.addSpecificScorePoints(player, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                            break;
                        case 1: //original wolf
                            GameManager.Instance.addSpecificScorePoints(player, GameManager.Instance.scoreValuesManagerScript.PointsSecondPlace);
                            break;
                    }
                }
                break;
        }
    }

    private void SpawnPlayerRandomly()
    {
        //randomNumbers = GenerateRandomNumbers(4, 0, 4);

        for (int i = 0; i < players.Length; i++)
        {
            players[i].transform.position = tpPoints[i].position;
        }
    }

    /*private void SpawnPlayerRandomlyWithWolf()
    {
        randomNumbers = GenerateRandomNumbers(3, 0, 3);

        for (int i = 0; i < playersNotWolf.Count; i++)
        {
            playersNotWolf[i].transform.position = tpPoints[randomNumbers[i]].position;
        }
        players[wolfPlayerNumber].transform.position = wolfTpPoint.position;
    }*/

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
