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

        } else if (nbPlayerInZone == 0)
        {
            timerBeforeTeleportationActu = 0;
            isTimerInitiated = false;
        }

        if (((Time.time >= timerBeforeTeleportationActu && isTimerInitiated && !isGameInitialized) || isDebug) && !isSendingToLobby)
        {
            isDebug = false; //reset

            isTimerInitiated = false;
            isGameInitialized = true; //for debug purpuses: play fnct only once
            //doesnt work
            GameManager.Instance.initializeGameModes(5);
        }
        else if (((Time.time >= timerBeforeTeleportationActu && isTimerInitiated && !isGameInitialized) || isDebug) && isSendingToLobby)
        {
            isDebug = false; //reset

            isTimerInitiated = false;
            isGameInitialized = true;
            SceneManager.LoadScene("Test"); //return to lobby
        }
    }
}
