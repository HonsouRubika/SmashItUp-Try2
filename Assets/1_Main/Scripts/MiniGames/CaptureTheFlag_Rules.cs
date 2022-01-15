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

    private bool playOneTime = false;

    private void Start()
    {
        playersUnsorted = GameObject.FindGameObjectsWithTag("Player");
        players = playersUnsorted.OrderBy(go => go.name).ToArray();

        //à toujours mettre dans le start
        AssignPlayerTeam();

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

        for (int i = 0; i < playersTeam.Length; i++)
        {
            players[i].GetComponent<PlayerSkins>().SetHammerColorByTeam("default");
        }

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

    private void AssignPlayerTeam()
    {
        /// TODO : Attribution al�atoire pour la compp des equipes 1 et 2
        //Debug.Log("In Game : " + GameManager.Instance.getTeamCompo());
        playersTeam = new int[players.Length];

        //verif si nb players insufisant
        int teamCompo = GameManager.Instance.getTeamCompo();
        if (players.Length <= 2 && teamCompo ==1 ) teamCompo = 0; //coop to 1v3
        if (players.Length <= 2 && teamCompo ==2 ) teamCompo = 3; //coop to 1v3

        for (int i = 0; i <players.Length; i++) 
        {
            switch (teamCompo)
            {
                case (int)GameManager.TeamCompo.FFA:
                    playersTeam[i] = i;
                    //Debug.Log("1v1v1v1");
                    //Debug.Log("In switch FFA");
                    //pas d'�quipe
                    break;
                case (int)GameManager.TeamCompo.Coop:
                    //Debug.Log("coop");
                    playersTeam[i] = 0;
                    //tous ensemble equipe 0
                    break;
                case (int)GameManager.TeamCompo.OneVSThree:
                    //Debug.Log("1v3");
                    GameManager.Instance.getMVP();
                    if (i == GameManager.Instance.getMVP()) playersTeam[i] = 0;
                    else playersTeam[i] = 1;
                    break;
                case (int)GameManager.TeamCompo.TwoVSTwo:
                    //Debug.Log("2v2");
                    if (i < 2) playersTeam[i] = 0;
                    else playersTeam[i] = 1;
                    break;
            }
        }
        //Debug.Log("in switch alea : " + playersTeam.Length);

        //aléa team players
        switch (teamCompo)
        {
            case (int)GameManager.TeamCompo.OneVSThree:
                /*
                    Debug.Log("1v3");
                    GameManager.Instance.getMVP();
                    if (i == GameManager.Instance.getMVP()) playersTeam[i] = 0;
                    else playersTeam[i] = 1;
                */
                    break;
            case (int)GameManager.TeamCompo.TwoVSTwo:
                for (int i = 0; i < playersTeam.Length; i++)
                {
                    int temp = playersTeam[i];
                    int randomIndex = Random.Range(i, playersTeam.Length);
                    playersTeam[i] = playersTeam[randomIndex];
                    playersTeam[randomIndex] = temp;
                    //Debug.Log(playersTeam[randomIndex]);
                }
                break;
        }

        if (teamCompo == 1 || teamCompo == 2)
        {
            for (int i = 0; i < playersTeam.Length; i++)
            {
                switch (playersTeam[i])
                {
                    case 0:
                        players[i].GetComponent<PlayerSkins>().SetHammerColorByTeam("purple");
                        break;
                    case 1:
                        players[i].GetComponent<PlayerSkins>().SetHammerColorByTeam("orange");
                        break;
                }
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
