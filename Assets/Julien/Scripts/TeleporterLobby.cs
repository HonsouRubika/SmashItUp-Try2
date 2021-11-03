using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterLobby : MonoBehaviour
{
    //public GameManager gameManager;
    public int nbPlayerInZone = 0;
    public int nbPlayerInGame = 0;

    public float timerBeforeTeleportation = 5;
    public float timerBeforeTeleportationActu = 0;
    private bool isTimerInitiated = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("player enter in tp zone");
        nbPlayerInZone++;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("player exit tp zone");
        nbPlayerInZone--;
    }

    private void Update()
    {
        if (nbPlayerInZone !=0 && nbPlayerInZone == nbPlayerInGame && !isTimerInitiated)
        {
            //start timer
            timerBeforeTeleportationActu = timerBeforeTeleportation + Time.time;
            isTimerInitiated = true;

        } else if (nbPlayerInZone == 0)
        {
            timerBeforeTeleportationActu = 0;
            isTimerInitiated = false;
        }

        if (Time.time >= timerBeforeTeleportationActu && isTimerInitiated)
        {
            Debug.Log("doit débuter le compte à rebour avant lancement du jeu");
        }
    }
}
