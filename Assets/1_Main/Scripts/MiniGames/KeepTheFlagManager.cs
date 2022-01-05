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

    public List<Transform> tpPoints = new List<Transform>();
    private List<int> randomNumbers = new List<int>();

    [Header("KeepFlag Rules")]
    public int winPoints;

    [Header("KeepFlag Score")]
    public bool player0HaveFlag = false;
    public bool player1HaveFlag = false;
    public bool player2HaveFlag = false;
    public bool player3HaveFlag = false;
    public float[] scorePlayers;
    private float[] finalScores;
    private int[] playersPosition;

    public int currentPlayerHaveFlag = 0;

    private bool playOneTime = false;

    //team compo
    public int[] playersTeam;

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

        finalScores = new float[players.Length];
        playersPosition = new int[players.Length];
        for (int i = 0; i < players.Length; i++)
        {
            playersPosition[i] = i;
        }

        SpawnPlayerRandomly();
        GameManager.Instance.focusPlayersScript.SetGameTitle("KeepTheFlag");

        AssignPlayerTeam();
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

        if (timerScript.miniGameTimer <= 0 && !playOneTime)
        {
            keepingFlagScript.ResetFlag();
            SortPlayers();

            playOneTime = true;
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
        for (int i = 0; i < finalScores.Length; i++)
        {
            finalScores[i] = scorePlayers[i];
        }

        System.Array.Sort(finalScores, playersPosition);

        // Point Distribution By Team Composition
        switch (GameManager.Instance.getTeamCompo())
        {
            case (int)GameManager.TeamCompo.FFA:
                switch (playersPosition.Length)
                {
                    case 4:
                        GameManager.Instance.addSpecificScorePoints(playersPosition[playersPosition.Length - 1] + 1, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                        GameManager.Instance.addSpecificScorePoints(playersPosition[playersPosition.Length - 2] + 1, GameManager.Instance.scoreValuesManagerScript.PointsSecondPlace);
                        GameManager.Instance.addSpecificScorePoints(playersPosition[playersPosition.Length - 3] + 1, GameManager.Instance.scoreValuesManagerScript.PointsThirdPlace);
                        GameManager.Instance.addSpecificScorePoints(playersPosition[playersPosition.Length - 4] + 1, GameManager.Instance.scoreValuesManagerScript.PointsFourthPlace);
                        break;
                    case 3:
                        GameManager.Instance.addSpecificScorePoints(playersPosition[playersPosition.Length - 1] + 1, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                        GameManager.Instance.addSpecificScorePoints(playersPosition[playersPosition.Length - 2] + 1, GameManager.Instance.scoreValuesManagerScript.PointsSecondPlace);
                        GameManager.Instance.addSpecificScorePoints(playersPosition[playersPosition.Length - 3] + 1, GameManager.Instance.scoreValuesManagerScript.PointsThirdPlace);
                        break;
                    case 2:
                        GameManager.Instance.addSpecificScorePoints(playersPosition[playersPosition.Length - 1] + 1, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                        GameManager.Instance.addSpecificScorePoints(playersPosition[playersPosition.Length - 2] + 1, GameManager.Instance.scoreValuesManagerScript.PointsSecondPlace);
                        break;
                }
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
                    if (playersTeam[i] == playersTeam[playersPosition[playersPosition.Length - 1]]) GameManager.Instance.addSpecificScorePoints(i + 1, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                    else GameManager.Instance.addSpecificScorePoints(i + 1, GameManager.Instance.scoreValuesManagerScript.PointsThirdPlace);
                }
                break;
            case (int)GameManager.TeamCompo.TwoVSTwo:
                for (int i = 0; i < players.Length; i++)
                {
                    if (playersTeam[i] == playersTeam[playersPosition[playersPosition.Length - 1]]) GameManager.Instance.addSpecificScorePoints(i + 1, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                    else GameManager.Instance.addSpecificScorePoints(i + 1, GameManager.Instance.scoreValuesManagerScript.PointsThirdPlace);
                }
                break;
        }

        playOneTime = true;
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

    private void AssignPlayerTeam()
    {
        /// TODO : Attribution al�atoire pour la compp des equipes 1 et 2
        //Debug.Log("In Game : " + GameManager.Instance.getTeamCompo());
        playersTeam = new int[players.Length];

        //verif si nb players insufisant
        int teamCompo = GameManager.Instance.getTeamCompo();
        if (players.Length <= 2 && teamCompo == 1) teamCompo = 0; //coop to 1v3
        if (players.Length <= 2 && teamCompo == 2) teamCompo = 3; //coop to 1v3

        for (int i = 0; i < players.Length; i++)
        {
            switch (teamCompo)
            {
                case (int)GameManager.TeamCompo.FFA:
                    playersTeam[i] = i;
                    Debug.Log("1v1v1v1");
                    //Debug.Log("In switch FFA");
                    //pas d'�quipe
                    break;
                case (int)GameManager.TeamCompo.Coop:
                    Debug.Log("coop");
                    playersTeam[i] = 0;
                    //tous ensemble equipe 0
                    break;
                case (int)GameManager.TeamCompo.OneVSThree:
                    Debug.Log("1v3");
                    if (i == 0) playersTeam[i] = 0;
                    else playersTeam[i] = 1;
                    break;
                case (int)GameManager.TeamCompo.TwoVSTwo:
                    Debug.Log("2v2");
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
                for (int i = 0; i < playersTeam.Length; i++)
                {
                    int temp = playersTeam[i];
                    int randomIndex = Random.Range(i, playersTeam.Length);
                    playersTeam[i] = playersTeam[randomIndex];
                    playersTeam[randomIndex] = temp;
                    //Debug.Log(playersTeam[randomIndex]);
                }
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
                        players[i].GetComponent<PlayerSkins>().SetColorByTeam("blue");
                        break;
                    case 1:
                        players[i].GetComponent<PlayerSkins>().SetColorByTeam("red");
                        break;
                }
            }
        }
    }
}
