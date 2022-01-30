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

    private bool playOneTime = false;

    private void Start()
    {
        playersUnsorted = GameObject.FindGameObjectsWithTag("Player");
        players = playersUnsorted.OrderBy(go => go.name).ToArray();

        //à toujours mettre dans le start
        playersTeam = GameManager.Instance.playersTeam;

        SpawnPlayerRandomly();
        GameManager.Instance.focusPlayersScript.SetGameTitle("CaptureTheFlag");
    }

    public void FlagCaptured(int playerWin)
    {
        // Point Distribution By Team Composition
        switch (GameManager.Instance.getTeamCompo())
        {
            case (int)GameManager.TeamCompo.FFA:
                GameManager.Instance.addSpecificScorePoints(playerWin+1, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                break;
            case (int)GameManager.TeamCompo.Coop:
                //if win
                GameManager.Instance.addScoresPoints(GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                //if loose
                //GameManager.Instance.addScores(0, 0, 0, 0);
                break;
            case (int)GameManager.TeamCompo.OneVSThree:
                for (int i = 0; i < players.Length; i++)
                {
                    if (playersTeam[i] == playersTeam[playerWin]) GameManager.Instance.addSpecificScorePoints(i+1, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                }
                break;
            case (int)GameManager.TeamCompo.TwoVSTwo:
                for (int i = 0; i < players.Length; i++)
                {
                    if (playersTeam[i] == playersTeam[playerWin]) GameManager.Instance.addSpecificScorePoints(i+1, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                }
                break;
        }

        //GameManager.Instance.NextMap();
        GameManager.Instance.Score();
        /*
        for (int i = 0; i < playersTeam.Length; i++)
        {
            players[i].GetComponent<PlayerSkins>().SetHammerColorByTeam("default");
        }
        */
        playOneTime = true;
    }

    /*public void FlagCaptured(int playerWin)
    {
        if (!playOneTime)
        {
            switch (playerWin)
            {
                case 0:
                    GameManager.Instance.addScoresPoints(GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace, 0, 0, 0);
                    break;
                case 1:
                    GameManager.Instance.addScoresPoints(0, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace, 0, 0);
                    break;
                case 2:
                    GameManager.Instance.addScoresPoints(0, 0, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace, 0);
                    break;
                case 3:
                    GameManager.Instance.addScoresPoints(0, 0, 0, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                    break;
            }

            //GameManager.Instance.NextMap();
            GameManager.Instance.Score();

            playOneTime = true;
        }
    }*/

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
