using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleporterLobby : MonoBehaviour
{
    public int nbPlayerInZone = 0;
    public int nbPlayerInGame = 0;

    public float timerBeforeTeleportation = 2;
    public float timerBeforeTeleportationActu = 0;
    private bool isTimerInitiated = false;
    private bool isGameInitialized = false;

    public bool isDebug;
    public bool isSendingToLobby = false;

    public StartGameSound StartGameSoundScript;
    public SortPlayersAfterGame sortPlayersScript;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            nbPlayerInZone++;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            nbPlayerInZone--;
            isTimerInitiated = false;
        }
    }

    private void Start()
    {
        isDebug = false;
    }

    private void Update()
    {
        if (nbPlayerInZone >= 2 && nbPlayerInZone == nbPlayerInGame && !isTimerInitiated)
        {
            //start timer
            timerBeforeTeleportationActu = timerBeforeTeleportation + Time.time;
            isTimerInitiated = true;

        } else if (nbPlayerInZone != nbPlayerInGame)
        {
            timerBeforeTeleportationActu = 0;
            isTimerInitiated = false;
        }

        if (((Time.time >= timerBeforeTeleportationActu && isTimerInitiated && !isGameInitialized) || isDebug) && !isSendingToLobby)
        {
            isDebug = false; //reset

            isTimerInitiated = false;
            isGameInitialized = true; //for debug purpuses: play fnct only once

            DestroyRewards();

            //doesnt work
            GameManager.Instance.initializeGameModes();

            StartGameSoundScript.GameStarting();
        }
        else if (((Time.time >= timerBeforeTeleportationActu && isTimerInitiated && !isGameInitialized) || isDebug) && isSendingToLobby)
        {
            isDebug = false; //reset

            isTimerInitiated = false;
            isGameInitialized = true;

            DestroyRewards();

            //nouveau skin débloquer
            if ((GameManager.Instance.nbGameFinished == 1 ||
               GameManager.Instance.nbGameFinished == 3 ||
               GameManager.Instance.nbGameFinished == 5) 
               && !isSendingToLobby)
            {
                SceneManager.LoadScene("NewSkinScene"); //return to lobby
            }
            else SceneManager.LoadScene("StartScene"); //return to lobby
        }
    }

    private void DestroyRewards()
    {
        if (sortPlayersScript != null)
        {
            if (sortPlayersScript.destroyCrownOnHub)
            {
                for (int i = 0; i < sortPlayersScript.crownInstances.Length; i++)
                {
                    Destroy(sortPlayersScript.crownInstances[i]);
                }
            }
            if (sortPlayersScript.destroyHammerOnHub)
            {
                sortPlayersScript.players[sortPlayersScript.players.Length - 1].GetComponent<PlayerSkins>().SetHammerColorByTeam("reset");
            }
        }
    }
}
