using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// By Antoine LEROUX
/// This script reference the rules of mini-game Capture the flag
/// </summary>

public class CaptureTheFlag_Rules : MonoBehaviour
{
    private GameObject[] playersUnsorted;
    public GameObject[] players;

    //team compo
    public int[] playersTeam;

    public List<Transform> tpPoints = new List<Transform>();
    private List<int> randomNumbers = new List<int>();

    [Header("CaptureTheFlag Rules")]
    public int winPoints;

    private void Start()
    {
        playersUnsorted = GameObject.FindGameObjectsWithTag("Player");
        players = playersUnsorted.OrderBy(go => go.name).ToArray();
        
        SpawnPlayerRandomly();
        GameManager.Instance.focusPlayersScript.SetGameTitle("CaptureTheFlag");
    }

    public void FlagCaptured(int playerWin)
    {
        //GameManager.Instance.getTeamCompo

        // Point Distribution By Team Composition
        switch (GameManager.Instance.getTeamCompo())
        {
            case (int)GameManager.TeamCompo.FFA:
                GameManager.Instance.addSpecificScore(playerWin, winPoints);
                break;
            case (int)GameManager.TeamCompo.Coop:
                //if win
                GameManager.Instance.addScores(winPoints, winPoints, winPoints, winPoints);
                //if loose
                //GameManager.Instance.addScores(0, 0, 0, 0);
                break;
            case (int)GameManager.TeamCompo.OneVSThree:
                for(int i = 0; i< players.Length; i++)
                {
                    if (playersTeam[i] == playersTeam[playerWin]) GameManager.Instance.addSpecificScore(i, winPoints);
                }
                break;
            case (int)GameManager.TeamCompo.TwoVSTwo:
                for (int i = 0; i < players.Length; i++)
                {
                    if (playersTeam[i] == playersTeam[playerWin]) GameManager.Instance.addSpecificScore(i, winPoints);
                }
                break;
        }

        /* OLD SWITCH Content
         * case 0:
                GameManager.Instance.addScores(winPoints, 0, 0, 0);
                break;
            case 1:
                GameManager.Instance.addScores(0, winPoints, 0, 0);
                break;
            case 2:
                GameManager.Instance.addScores(0, 0, winPoints, 0);
                break;
            case 3:
                GameManager.Instance.addScores(0, 0, 0, winPoints);
                break;
        */

        //GameManager.Instance.NextMap();
        GameManager.Instance.Score();
    }

    private void AssignPlayerTeam()
    {
        /// TODO : Attribution aléatoire pour la compp des equipes 1 et 2

        for (int i = 0; i <players.Length; i++) 
        {
            switch (GameManager.Instance.getTeamCompo())
            {
                case (int)GameManager.TeamCompo.FFA:
                    playersTeam[i] = i;
                    //pas d'équipe
                    break;
                case (int)GameManager.TeamCompo.Coop:
                    playersTeam[i] = 0;
                    //tous ensemble equipe 0
                    break;
                case (int)GameManager.TeamCompo.OneVSThree:
                    if (i == 0) playersTeam[i] = 0;
                    else playersTeam[i] = 1;
                    break;
                case (int)GameManager.TeamCompo.TwoVSTwo:
                    if (i <= 2) playersTeam[i] = 0;
                    else playersTeam[i] = 1;
                    break;
            }
        }
        
    }

    private void SpawnPlayerRandomly()
    {
        randomNumbers = GenerateRandomNumbers(4, 0, 4);

        for (int i = 0; i < players.Length; i++)
        {
            players[i].transform.position = tpPoints[randomNumbers[i]].position;
        }
    }

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
