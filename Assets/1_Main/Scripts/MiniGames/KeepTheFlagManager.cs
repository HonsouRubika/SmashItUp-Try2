using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class KeepTheFlagManager : MonoBehaviour
{
    private GameObject[] playersUnsorted;
    public GameObject[] players;
    private PlayerController[] playersControllers;

    private KeepingFlag keepingFlagScript;

    [Header("UI")]
    public Score scoreScript;
    public Timer timerScript;

    [Header("TP Points")]
    public Transform tpPoints0;
    public Transform tpPoints1;
    public Transform tpPoints2;
    public Transform tpPoints3;

    [Header("KeepFlag Rules")]
    public int winPoints;

    [Header("KeepFlag Score")]
    public bool player0HaveFlag = false;
    public bool player1HaveFlag = false;
    public bool player2HaveFlag = false;
    public bool player3HaveFlag = false;
    public float[] scorePlayers;
    public float[] finalScores;

    public int currentPlayerHaveFlag = 0;

    private void Start()
    {
        keepingFlagScript = GetComponentInChildren<KeepingFlag>();

        playersUnsorted = GameObject.FindGameObjectsWithTag("Player");
        players = playersUnsorted.OrderBy(go => go.name).ToArray();

        playersControllers = new PlayerController[players.Length];
        scorePlayers = new float[players.Length];
        for (int i = 0; i < players.Length; i++)
        {
            playersControllers[i] = players[i].GetComponent<PlayerController>();
        }

        players[0].transform.position = tpPoints0.position;
        players[1].transform.position = tpPoints1.position;
        players[2].transform.position = tpPoints2.position;
        players[3].transform.position = tpPoints3.position;
    }

    private void Update()
    {
        IncrementPlayerScore();

        for (int i = 0; i < playersControllers.Length; i++)
        {
            if (playersControllers[i].hitPlayer)
            {
                if (playersControllers[i].playerIDHit == currentPlayerHaveFlag - 1)     //Attacked the player who have the flag
                {
                    FlagCaptured((int)playersControllers[i].playerID);
                    keepingFlagScript.AttachFlagToPlayer(players[(int)playersControllers[i].playerID].transform);
                }
            }
        }

        if (timerScript.miniGameTimer <= 0)
        {
            SortPlayers();
        }
    }

    public void FlagCaptured(int player)
    {
        switch (player)
        {
            case 0:
                player0HaveFlag = true;
                player1HaveFlag = false;
                player2HaveFlag = false;
                player3HaveFlag = false;
                break;
            case 1:
                player0HaveFlag = false;
                player1HaveFlag = true;
                player2HaveFlag = false;
                player3HaveFlag = false;
                break;
            case 2:
                player0HaveFlag = false;
                player1HaveFlag = false;
                player2HaveFlag = true;
                player3HaveFlag = false;
                break;
            case 3:
                player0HaveFlag = false;
                player1HaveFlag = false;
                player2HaveFlag = false;
                player3HaveFlag = true;
                break;
        }
    }

    private void IncrementPlayerScore()
    {
        if (scorePlayers.Length == 2)
        {
            scoreScript.SetScore((int)scorePlayers[0], (int)scorePlayers[1], 0, 0);
        }
        else if (scorePlayers.Length == 3)
        {
            scoreScript.SetScore((int)scorePlayers[0], (int)scorePlayers[1], (int)scorePlayers[2], 0);
        }
        else if (scorePlayers.Length == 4)
        {
            scoreScript.SetScore((int)scorePlayers[0], (int)scorePlayers[1], (int)scorePlayers[2], (int)scorePlayers[3]);
        }

        if (player0HaveFlag)
        {
            scorePlayers[0] += Time.deltaTime;
            currentPlayerHaveFlag = 1;
        }
        else if (player1HaveFlag)
        {
            scorePlayers[1] += Time.deltaTime;
            currentPlayerHaveFlag = 2;
        }
        else if (player2HaveFlag)
        {
            scorePlayers[2] += Time.deltaTime;
            currentPlayerHaveFlag = 3;
        }
        else if (player3HaveFlag)
        {
            scorePlayers[3] += Time.deltaTime;
            currentPlayerHaveFlag = 4;
        }
    }

    private void SortPlayers()
    {
        finalScores = scorePlayers.OrderBy(f => f).ToArray();

        //GameManager.Instance.addScores(0, 10, 10, 10);
        //GameManager.Instance.addSpecificScore(player, 10);
    }
}
