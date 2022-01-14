using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class PlayerManagerScript : MonoBehaviour
{
    public TeleporterLobby teleporterLobby;
    public TeleporterLobby teleporterRestartGame;
    public Transform spawner1, spawner2, spawner3, spawner4;
    public GameObject door1, door2, door3, door4;
    private int nbPlayerActu = 0;

    public bool playerPositionByScore = false;

    private GameObject[] playersUnsorted;
    public GameObject[] players;
    public int[] finalPlayerScore;

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

        if (playerPositionByScore)
        {
            SpawnPlayerByScore();
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
}
