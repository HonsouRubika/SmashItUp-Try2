using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureManager : MonoBehaviour
{
    public int scorePlayer0 = 0;
    public int scorePlayer1 = 0;
    private Zone zoneScript;
    public float timePastInZone = 0;
    public float timeToScore = 1;
    private CaptureSound captureSoundScript;


    private void Start()
    {
        zoneScript = GetComponentInChildren<Zone>();

        captureSoundScript = GetComponentInChildren<CaptureSound>();

    }

    private void Update()
    {
        if (zoneScript.player0IsInZone == true && zoneScript.player1IsInZone == true)
        {
            timePastInZone = 0;
            captureSoundScript.PlayerOutZone();


        }
        else
        {
            if (zoneScript.player0IsInZone == true)
            {
                timePastInZone += Time.deltaTime;
                captureSoundScript.PlayerCapturing();


                if (timePastInZone >= timeToScore)
                {
                    scorePlayer0++;
                    timePastInZone = 0;
                }
            }          

            if (zoneScript.player1IsInZone == true)
            {
                timePastInZone += Time.deltaTime;
                captureSoundScript.PlayerCapturing();

                if (timePastInZone >= timeToScore)
                {
                    scorePlayer1++;
                    timePastInZone = 0;
                }
            }            
        }

        if (zoneScript.player0IsInZone == false && zoneScript.player1IsInZone == false)
        {
            timePastInZone = 0;
            captureSoundScript.PlayerOutZone();
        }
    }
}

// Il faut trouver un moyen de remettre le timer de capture à 0 quand un joueur quitte la zone, même si un autre joueur est dedans
// Il manque le timer du mini jeu (le global) 
// Un scoring avec un classement pour chaque joueur (if score0 < score1 alors Joueur1 est premier et Joueur0 est deuxième)