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

    private int nbPlayerActu = 0;

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        //initialisation des joueurs dans le menu selection
        if (teleporterLobby != null) teleporterLobby.nbPlayerInGame++;
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
}
