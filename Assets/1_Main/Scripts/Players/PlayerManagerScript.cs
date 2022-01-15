using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class PlayerManagerScript : MonoBehaviour
{
    public TeleporterLobby teleporterLobby;
    public TeleporterLobby teleporterRestartGame;

    [Header("spawner")]
    public Transform spawner1;
    public Transform spawner2;
    public Transform spawner3;
    public Transform spawner4;
    //public GameObject door1, door2, door3, door4;

    [Header("crow")]
    public GameObject crownOfWinner;
    [HideInInspector] public GameObject[] crownInstances;
    public float flagAttachedPlayerPosY;

    private int nbPlayerActu = 0;

    public bool playerPositionByScore = false;

    private GameObject[] playersUnsorted;
    public GameObject[] players;
    public int[] finalPlayerScore;

    //Equality
    public WinnerEqualityCase winnerEqualityCase = WinnerEqualityCase.None;

    private bool allScoreZero = true;

    public void Start()
    {
        playersUnsorted = GameObject.FindGameObjectsWithTag("Player");
        players = playersUnsorted.OrderBy(go => go.name).ToArray();
        finalPlayerScore = new int[players.Length];

        switch (players.Length)
        {
            case 2:
                finalPlayerScore[0] = GameManager.Instance._scoreP1;
                finalPlayerScore[1] = GameManager.Instance._scoreP2;
                break;
            case 3:
                finalPlayerScore[0] = GameManager.Instance._scoreP1;
                finalPlayerScore[1] = GameManager.Instance._scoreP2;
                finalPlayerScore[2] = GameManager.Instance._scoreP3;
                break;
            case 4:
                finalPlayerScore[0] = GameManager.Instance._scoreP1;
                finalPlayerScore[1] = GameManager.Instance._scoreP2;
                finalPlayerScore[2] = GameManager.Instance._scoreP3;
                finalPlayerScore[3] = GameManager.Instance._scoreP4;
                break;
        }

        System.Array.Sort(finalPlayerScore, players);

        CheckIfEquality();

        if (playerPositionByScore)
        {
            SpawnPlayerByScore();
            AttachCrownToPlayer();
        }
    }

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        //initialisation des joueurs dans le menu selection
        teleporterLobby.nbPlayerInGame++;
        if (teleporterRestartGame != null) teleporterRestartGame.nbPlayerInGame++;

        //instancialisation dans joueurs au d�but de chaque mapDebug.Log("player connected");
        switch (nbPlayerActu)
        {
            case 0:
                playerInput.transform.position = new Vector2(spawner1.position.x, spawner1.position.y);
                playerInput.GetComponent<PlayerController>().playerID = 0;
                //door1.SetActive(false);

                DontDestroyOnLoad(playerInput);
                break;
            case 1:
                playerInput.transform.position = new Vector2(spawner2.position.x, spawner2.position.y);
                playerInput.GetComponent<PlayerController>().playerID = 1;
                //door2.SetActive(false);

                DontDestroyOnLoad(playerInput);
                break;
            case 2:
                playerInput.transform.position = new Vector2(spawner3.position.x, spawner3.position.y);
                playerInput.GetComponent<PlayerController>().playerID = 2;
                //door3.SetActive(false);

                DontDestroyOnLoad(playerInput);
                break;
            case 3:
                playerInput.transform.position = new Vector2(spawner4.position.x, spawner4.position.y);
                playerInput.GetComponent<PlayerController>().playerID = 3;
                //door4.SetActive(false);

                DontDestroyOnLoad(playerInput);
                break;
        }
        nbPlayerActu++;
    }

    public void OnPlayerLeft(PlayerInput playerInput)
    {
        //A v�rifier
        teleporterLobby.nbPlayerInGame++;
        if (teleporterRestartGame != null) teleporterRestartGame.nbPlayerInGame--;
        nbPlayerActu--;
    }

    private void SpawnPlayerByScore()
    {
        switch (players.Length)
        {
            case 2:
                players[0].transform.position = spawner2.position;
                players[1].transform.position = spawner1.position;

                break;
            case 3:
                players[0].transform.position = spawner3.position;
                players[1].transform.position = spawner2.position;
                players[2].transform.position = spawner1.position;
                break;
            case 4:
                players[0].transform.position = spawner4.position;
                players[1].transform.position = spawner3.position;
                players[2].transform.position = spawner2.position;
                players[3].transform.position = spawner1.position;
                break;
        }     
    }

    public void AttachCrownToPlayer()
    {
        switch (winnerEqualityCase)
        {
            case WinnerEqualityCase.None:
                crownInstances = new GameObject[1];
                crownInstances[0] = Instantiate(crownOfWinner, new Vector2(players[players.Length - 1].transform.position.x, players[players.Length - 1].transform.position.y + flagAttachedPlayerPosY), Quaternion.identity);
                crownInstances[0].transform.SetParent(players[players.Length - 1].transform);
                break;
            case WinnerEqualityCase.AllEqualDifferentZero:
                crownInstances = new GameObject[4];
                for (int i = 0; i < crownInstances.Length; i++)
                {
                    crownInstances[i] = Instantiate(crownOfWinner, new Vector2(players[players.Length - 1 - i].transform.position.x, players[players.Length - 1 - i].transform.position.y + flagAttachedPlayerPosY), Quaternion.identity);
                    crownInstances[i].transform.SetParent(players[players.Length - 1 - i].transform);
                }
                break;
            case WinnerEqualityCase.FirstSecondThird:
                crownInstances = new GameObject[3];
                for (int i = 0; i < crownInstances.Length; i++)
                {
                    crownInstances[i] = Instantiate(crownOfWinner, new Vector2(players[players.Length - 1 - i].transform.position.x, players[players.Length - 1 - i].transform.position.y + flagAttachedPlayerPosY), Quaternion.identity);
                    crownInstances[i].transform.SetParent(players[players.Length - 1 - i].transform);
                }
                break;
            case WinnerEqualityCase.FirstSecond:
                crownInstances = new GameObject[2];
                for (int i = 0; i < crownInstances.Length; i++)
                {
                    crownInstances[i] = Instantiate(crownOfWinner, new Vector2(players[players.Length - 1 - i].transform.position.x, players[players.Length - 1 - i].transform.position.y + flagAttachedPlayerPosY), Quaternion.identity);
                    crownInstances[i].transform.SetParent(players[players.Length - 1 - i].transform);
                }
                break;
        }
    }

    private void CheckIfEquality()
    {
        //Check if no players have points
        for (int i = 0; i < finalPlayerScore.Length; i++)
        {
            if (finalPlayerScore[i] != 0)
            {
                allScoreZero = false;
                break;
            }
        }

        //Check if equality between players
        for (int i = 0; i < finalPlayerScore.Length; i++)
        {
            if (finalPlayerScore.Length == 4)
            {
                if (i == 3)
                {
                    if (finalPlayerScore[i] == finalPlayerScore[i - 1])
                    {
                        //equality between 1st & 2nd
                        winnerEqualityCase = WinnerEqualityCase.FirstSecond;
                        //Debug.Log("equality between 1st & 2nd");
                    }
                }
                else if (i == 1)
                {
                    if (finalPlayerScore[i] == finalPlayerScore[i + 2] && finalPlayerScore[i] == finalPlayerScore[i - 1])
                    {
                        if (allScoreZero)
                        {
                            //equality between all players
                            winnerEqualityCase = WinnerEqualityCase.AllEqual;
                            //Debug.Log("equality between all players");
                            break;
                        }
                        else
                        {
                            //equality between all players different from 0
                            winnerEqualityCase = WinnerEqualityCase.AllEqualDifferentZero;
                            //Debug.Log("equality between all players different from 0");
                            break;
                        }
                    }
                    else if (finalPlayerScore[i] == finalPlayerScore[i + 2])
                    {
                        //equality between 1st & 2nd & 3rd
                        winnerEqualityCase = WinnerEqualityCase.FirstSecondThird;
                        //Debug.Log("equality between 1st & 2nd & 3rd");
                        break;
                    }
                }
            }
            else if (finalPlayerScore.Length == 3)
            {
                if (i == 1)
                {
                    if (finalPlayerScore[i] == finalPlayerScore[i - 1] && finalPlayerScore[i] == finalPlayerScore[i + 1])
                    {
                        if (allScoreZero)
                        {
                            //equality between all players
                            winnerEqualityCase = WinnerEqualityCase.AllEqual;
                            //Debug.Log("equality between all players");
                            break;
                        }
                        else
                        {
                            //equality between all players different from 0
                            winnerEqualityCase = WinnerEqualityCase.AllEqualDifferentZero;
                            //Debug.Log("equality between all players different from 0");
                            break;
                        }
                    }
                    else if (finalPlayerScore[i] == finalPlayerScore[i + 1])
                    {
                        //equality between 1st & 2nd
                        winnerEqualityCase = WinnerEqualityCase.FirstSecond;
                        //Debug.Log("equality between 1st & 2nd");
                    }
                }
            }
            else if (finalPlayerScore.Length == 2)
            {
                if (i == 1)
                {
                    if (finalPlayerScore[i] == finalPlayerScore[i - 1])
                    {
                        if (allScoreZero)
                        {
                            //equality between all players
                            winnerEqualityCase = WinnerEqualityCase.AllEqual;
                            //Debug.Log("equality between all players");
                            break;
                        }
                        else
                        {
                            //equality between all players different from 0
                            winnerEqualityCase = WinnerEqualityCase.AllEqualDifferentZero;
                            //Debug.Log("equality between all players different from 0");
                            break;
                        }
                    }
                }
            }
        }
    }

    public enum WinnerEqualityCase
    {
        None,
        AllEqual,
        AllEqualDifferentZero,
        FirstSecondThird,
        FirstSecond
    }
}
