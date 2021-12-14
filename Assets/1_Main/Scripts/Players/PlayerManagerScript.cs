using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManagerScript : MonoBehaviour
{
    public TeleporterLobby teleporterLobby;
    public TeleporterLobby teleporterRestartGame;
    public Transform spawner1, spawner2, spawner3, spawner4;
    private int nbPlayerActu = 0;

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

                DontDestroyOnLoad(playerInput);
                break;
            case 1:
                playerInput.transform.position = new Vector2(spawner2.position.x, spawner2.position.y);
                playerInput.GetComponent<PlayerController>().playerID = 1;

                DontDestroyOnLoad(playerInput);
                break;
            case 2:
                playerInput.transform.position = new Vector2(spawner3.position.x, spawner3.position.y);
                playerInput.GetComponent<PlayerController>().playerID = 2;

                DontDestroyOnLoad(playerInput);
                break;
            case 3:
                playerInput.transform.position = new Vector2(spawner4.position.x, spawner4.position.y);
                playerInput.GetComponent<PlayerController>().playerID = 3;

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
