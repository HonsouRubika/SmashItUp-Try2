using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class KeepTheFlagManager : MonoBehaviour
{
    private GameObject[] playersUnsorted;
    public GameObject[] players;
    private PlayerController[] playersControllers;

    private KeepingFlag keepingFlagScript;

    [Header("UI")]
    public Score scoreScriptTeam;
    public Score scoreScript;
    public Timer timerScript;

    public List<Transform> tpPoints = new List<Transform>();
    private List<int> randomNumbers = new List<int>();

    [Header("KeepFlag Rules")]
    public float timeToScore = 1;
    public int pointsEarned1V3Alone = 2;

    [Header("KeepFlag Score")]
    public bool player0HaveFlag = false;
    public bool player1HaveFlag = false;
    public bool player2HaveFlag = false;
    public bool player3HaveFlag = false;

    [HideInInspector] public float[] scorePlayers;

    public float[] timePastInZonePlayer;
    private float[] finalScores;
    private int[] playersPosition;

    [Header("Score")]
    public GameObject floatingPoint;
    public Vector2 spawnPointOffset;
    private GameObject instFloatingPoint;
    public Color player0Color;
    public Color player1Color;
    public Color player2Color;
    public Color player3Color;
    private int pointCounterP1 = 1;
    private int pointCounterP2 = 1;
    private int pointCounterP3 = 1;
    private int pointCounterP4 = 1;

    public int currentPlayerHaveFlag = 0;

    private bool playOneTime = false;

    private bool allScoreZero = true;

    //team compo
    public int[] playersTeam;
    private int playerAlone1v3 = 0;
    private bool OneVSThreeEnable = false;

    //Equality
    public EqualityCase equalityCase = EqualityCase.None;

    //Merge score team
    private float scoreTeam1;
    private float scoreTeam2;

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

        timePastInZonePlayer = new float[players.Length];
        for (int i = 0; i < timePastInZonePlayer.Length; i++)
        {
            timePastInZonePlayer[i] = timeToScore;
        }

        SpawnPlayerRandomly();
        GameManager.Instance.focusPlayersScript.SetGameTitle("KeepTheFlag");

        //à toujours mettre dans le start
        playersTeam = GameManager.Instance.playersTeam;

        if (GameManager.Instance.getTeamCompo() == 1)
        {
            OneVSThreeEnable = true;
            playerAlone1v3 = System.Array.IndexOf(playersTeam, 0);
        }
    }

    private void Update()
    {
        DisplayUITeam();

        if (timerScript.miniGameTimer > 0)
        {
            IncrementPlayerScore();
        }

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
            EqualizeScoreIfTeam();
            SortPlayers();

            ResetColor();

            playOneTime = true;
        }
    }

    private void DisplayUITeam()
    {
        if (GameManager.Instance.getTeamCompo() == 2 || GameManager.Instance.getTeamCompo() == 1)
        {
            scoreScriptTeam.transform.GetChild(0).gameObject.SetActive(true);
            scoreScript.transform.GetChild(0).gameObject.SetActive(false);

            scoreScriptTeam.EnableAddScore();
            scoreScriptTeam.SetScore(0, (int)scoreTeam2, (int)scoreTeam1, 0);
        }
        else
        {
            scoreScriptTeam.transform.GetChild(0).gameObject.SetActive(false);
            scoreScript.transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    private void MergeScoreTeam(int player, float score)
    {
        //2v2
        if (GameManager.Instance.getTeamCompo() == 2 || GameManager.Instance.getTeamCompo() == 1)
        {
            switch (playersTeam[player])
            {
                case 0:
                    scoreTeam1 += score;
                    break;
                case 1:
                    scoreTeam2 += score;
                    break;
            }
        }
    }

    private void EqualizeScoreIfTeam()
    {
        //2v2
        if (GameManager.Instance.getTeamCompo() == 2 || GameManager.Instance.getTeamCompo() == 1)
        {
            for (int i = 0; i < players.Length; i++)
            {
                switch (playersTeam[i])
                {
                    case 0:
                        scorePlayers[i] = scoreTeam1;
                        break;
                    case 1:
                        scorePlayers[i] = scoreTeam2;
                        break;
                }
            }
        }
    }

    private void ResetColor()
    {
        for (int i = 0; i < playersTeam.Length; i++)
        {
            players[i].GetComponent<PlayerSkins>().SetHammerColorByTeam("default");
            players[i].GetComponent<PlayerSkins>().SetCursorTeam("default");
            players[i].GetComponent<PlayerSkins>().SetHaloTeam("default");
        }
    }

    private void SpawnFloatingText(Color playerColor, GameObject player, int counter)
    {
        if (instFloatingPoint != null) Destroy(instFloatingPoint);
        instFloatingPoint = Instantiate(floatingPoint, new Vector2(player.transform.position.x + spawnPointOffset.x, player.transform.position.y + spawnPointOffset.y), Quaternion.identity, player.transform);
        instFloatingPoint.transform.GetChild(0).GetComponent<TextMeshPro>().text = "+ " + counter.ToString();
        instFloatingPoint.transform.GetChild(0).GetComponent<TextMeshPro>().color = playerColor;
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
        if (player0HaveFlag)
        {
            currentPlayerHaveFlag = 1;

            if (timePastInZonePlayer[0] >= timeToScore)
            {
                if (OneVSThreeEnable && playerAlone1v3 == 0)
                {
                    scorePlayers[0] += pointsEarned1V3Alone;
                    scoreScript.AddScore(pointsEarned1V3Alone, 0, 0, 0);
                    pointCounterP1 += pointsEarned1V3Alone;
                    MergeScoreTeam(0, pointsEarned1V3Alone);

                }
                else
                {
                    scorePlayers[0]++;
                    scoreScript.AddScore(1, 0, 0, 0);
                    pointCounterP1++;
                    MergeScoreTeam(0, 1);
                }

                timePastInZonePlayer[0] = 0;
                SpawnFloatingText(player0Color, players[0], pointCounterP1);
            }
            else
            {
                timePastInZonePlayer[0] += Time.deltaTime;
            }
        }
        else
        {
            if (timePastInZonePlayer.Length >= 1)
            {
                timePastInZonePlayer[0] = timeToScore;
                pointCounterP1 = 0;
            } 
        }

        if (player1HaveFlag)
        {
            currentPlayerHaveFlag = 2;

            if (timePastInZonePlayer[1] >= timeToScore)
            {
                if (OneVSThreeEnable && playerAlone1v3 == 1)
                {
                    scorePlayers[1] += pointsEarned1V3Alone;
                    scoreScript.AddScore(0, pointsEarned1V3Alone, 0, 0);
                    pointCounterP2 += pointsEarned1V3Alone;
                    MergeScoreTeam(1, pointsEarned1V3Alone);

                }
                else
                {
                    scorePlayers[1]++;
                    scoreScript.AddScore(0, 1, 0, 0);
                    pointCounterP2++;
                    MergeScoreTeam(1, 1);
                }

                timePastInZonePlayer[1] = 0;
                SpawnFloatingText(player1Color, players[1], pointCounterP2);
            }
            else
            {
                timePastInZonePlayer[1] += Time.deltaTime;
            }
        }
        else
        {
            if (timePastInZonePlayer.Length >= 2)
            {
                timePastInZonePlayer[1] = timeToScore;
                pointCounterP2 = 0;
            }
        }

        if (player2HaveFlag)
        {
            currentPlayerHaveFlag = 3;

            if (timePastInZonePlayer[2] >= timeToScore)
            {
                if (OneVSThreeEnable && playerAlone1v3 == 2)
                {
                    scorePlayers[2] += pointsEarned1V3Alone;
                    scoreScript.AddScore(0, 0, pointsEarned1V3Alone, 0);
                    pointCounterP3 += pointsEarned1V3Alone;
                    MergeScoreTeam(2, pointsEarned1V3Alone);

                }
                else
                {
                    scorePlayers[2]++;
                    scoreScript.AddScore(0, 0, 1, 0);
                    pointCounterP3++;
                    MergeScoreTeam(2, 1);
                }

                timePastInZonePlayer[2] = 0;
                SpawnFloatingText(player2Color, players[2], pointCounterP3);
            }
            else
            {
                timePastInZonePlayer[2] += Time.deltaTime;
            }
        }
        else
        {
            if (timePastInZonePlayer.Length >= 3)
            {
                timePastInZonePlayer[2] = timeToScore;
                pointCounterP3 = 0;
            }
        }

        if (player3HaveFlag)
        {
            currentPlayerHaveFlag = 4;

            if (timePastInZonePlayer[3] >= timeToScore)
            {
                if (OneVSThreeEnable && playerAlone1v3 == 3)
                {
                    scorePlayers[3] += pointsEarned1V3Alone;
                    scoreScript.AddScore(0, 0, 0, pointsEarned1V3Alone);
                    pointCounterP4 += pointsEarned1V3Alone;
                    MergeScoreTeam(3, pointsEarned1V3Alone);

                }
                else
                {
                    scorePlayers[3]++;
                    scoreScript.AddScore(0, 0, 0, 1);
                    pointCounterP4++;
                    MergeScoreTeam(3, 1);
                }

                timePastInZonePlayer[3] = 0;
                SpawnFloatingText(player3Color, players[3], pointCounterP4);
            }
            else
            {
                timePastInZonePlayer[3] += Time.deltaTime;
            }
        }
        else
        {
            if (timePastInZonePlayer.Length >= 4)
            {
                timePastInZonePlayer[3] = timeToScore;
                pointCounterP4 = 0;
            }
        }
    }

    private void SortPlayers()
    {
        for (int i = 0; i < finalScores.Length; i++)
        {
            finalScores[i] = scorePlayers[i];
        }

        System.Array.Sort(finalScores, playersPosition);

        CheckIfEquality();

        // Point Distribution By Team Composition
        switch (GameManager.Instance.getTeamCompo())
        {
            case (int)GameManager.TeamCompo.FFA:
                switch (playersPosition.Length)
                {
                    case 4:
                        switch (equalityCase)
                        {
                            case EqualityCase.None:
                                GameManager.Instance.addSpecificScorePoints(playersPosition[playersPosition.Length - 1] + 1, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                                GameManager.Instance.addSpecificScorePoints(playersPosition[playersPosition.Length - 2] + 1, GameManager.Instance.scoreValuesManagerScript.PointsSecondPlace);
                                GameManager.Instance.addSpecificScorePoints(playersPosition[playersPosition.Length - 3] + 1, GameManager.Instance.scoreValuesManagerScript.PointsThirdPlace);
                                GameManager.Instance.addSpecificScorePoints(playersPosition[playersPosition.Length - 4] + 1, GameManager.Instance.scoreValuesManagerScript.PointsFourthPlace);
                                break;
                            case EqualityCase.AllEqualDifferentZero:
                                GameManager.Instance.addSpecificScorePoints(playersPosition[playersPosition.Length - 1] + 1, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                                GameManager.Instance.addSpecificScorePoints(playersPosition[playersPosition.Length - 2] + 1, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                                GameManager.Instance.addSpecificScorePoints(playersPosition[playersPosition.Length - 3] + 1, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                                GameManager.Instance.addSpecificScorePoints(playersPosition[playersPosition.Length - 4] + 1, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                                break;
                            case EqualityCase.FirstSecondAndThirdFourth:
                                GameManager.Instance.addSpecificScorePoints(playersPosition[playersPosition.Length - 1] + 1, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                                GameManager.Instance.addSpecificScorePoints(playersPosition[playersPosition.Length - 2] + 1, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                                GameManager.Instance.addSpecificScorePoints(playersPosition[playersPosition.Length - 3] + 1, GameManager.Instance.scoreValuesManagerScript.PointsSecondPlace);
                                GameManager.Instance.addSpecificScorePoints(playersPosition[playersPosition.Length - 4] + 1, GameManager.Instance.scoreValuesManagerScript.PointsSecondPlace);
                                break;
                            case EqualityCase.FirstSecondThird:
                                GameManager.Instance.addSpecificScorePoints(playersPosition[playersPosition.Length - 1] + 1, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                                GameManager.Instance.addSpecificScorePoints(playersPosition[playersPosition.Length - 2] + 1, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                                GameManager.Instance.addSpecificScorePoints(playersPosition[playersPosition.Length - 3] + 1, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                                GameManager.Instance.addSpecificScorePoints(playersPosition[playersPosition.Length - 4] + 1, GameManager.Instance.scoreValuesManagerScript.PointsSecondPlace);
                                break;
                            case EqualityCase.SecondThirdFourth:
                                GameManager.Instance.addSpecificScorePoints(playersPosition[playersPosition.Length - 1] + 1, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                                GameManager.Instance.addSpecificScorePoints(playersPosition[playersPosition.Length - 2] + 1, GameManager.Instance.scoreValuesManagerScript.PointsSecondPlace);
                                GameManager.Instance.addSpecificScorePoints(playersPosition[playersPosition.Length - 3] + 1, GameManager.Instance.scoreValuesManagerScript.PointsSecondPlace);
                                GameManager.Instance.addSpecificScorePoints(playersPosition[playersPosition.Length - 4] + 1, GameManager.Instance.scoreValuesManagerScript.PointsSecondPlace);
                                break;
                            case EqualityCase.FirstSecond:
                                GameManager.Instance.addSpecificScorePoints(playersPosition[playersPosition.Length - 1] + 1, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                                GameManager.Instance.addSpecificScorePoints(playersPosition[playersPosition.Length - 2] + 1, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                                GameManager.Instance.addSpecificScorePoints(playersPosition[playersPosition.Length - 3] + 1, GameManager.Instance.scoreValuesManagerScript.PointsSecondPlace);
                                GameManager.Instance.addSpecificScorePoints(playersPosition[playersPosition.Length - 4] + 1, GameManager.Instance.scoreValuesManagerScript.PointsThirdPlace);
                                break;
                            case EqualityCase.SecondThird:
                                GameManager.Instance.addSpecificScorePoints(playersPosition[playersPosition.Length - 1] + 1, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                                GameManager.Instance.addSpecificScorePoints(playersPosition[playersPosition.Length - 2] + 1, GameManager.Instance.scoreValuesManagerScript.PointsSecondPlace);
                                GameManager.Instance.addSpecificScorePoints(playersPosition[playersPosition.Length - 3] + 1, GameManager.Instance.scoreValuesManagerScript.PointsSecondPlace);
                                GameManager.Instance.addSpecificScorePoints(playersPosition[playersPosition.Length - 4] + 1, GameManager.Instance.scoreValuesManagerScript.PointsThirdPlace);
                                break;
                            case EqualityCase.ThirdFourth:
                                GameManager.Instance.addSpecificScorePoints(playersPosition[playersPosition.Length - 1] + 1, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                                GameManager.Instance.addSpecificScorePoints(playersPosition[playersPosition.Length - 2] + 1, GameManager.Instance.scoreValuesManagerScript.PointsSecondPlace);
                                GameManager.Instance.addSpecificScorePoints(playersPosition[playersPosition.Length - 3] + 1, GameManager.Instance.scoreValuesManagerScript.PointsThirdPlace);
                                GameManager.Instance.addSpecificScorePoints(playersPosition[playersPosition.Length - 4] + 1, GameManager.Instance.scoreValuesManagerScript.PointsThirdPlace);
                                break;
                        }
                        break;
                    case 3:
                        switch (equalityCase)
                        {
                            case EqualityCase.None:
                                GameManager.Instance.addSpecificScorePoints(playersPosition[playersPosition.Length - 1] + 1, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                                GameManager.Instance.addSpecificScorePoints(playersPosition[playersPosition.Length - 2] + 1, GameManager.Instance.scoreValuesManagerScript.PointsSecondPlace);
                                GameManager.Instance.addSpecificScorePoints(playersPosition[playersPosition.Length - 3] + 1, GameManager.Instance.scoreValuesManagerScript.PointsThirdPlace);
                                break;
                            case EqualityCase.AllEqualDifferentZero:
                                GameManager.Instance.addSpecificScorePoints(playersPosition[playersPosition.Length - 1] + 1, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                                GameManager.Instance.addSpecificScorePoints(playersPosition[playersPosition.Length - 2] + 1, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                                GameManager.Instance.addSpecificScorePoints(playersPosition[playersPosition.Length - 3] + 1, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                                break;
                            case EqualityCase.FirstSecond:
                                GameManager.Instance.addSpecificScorePoints(playersPosition[playersPosition.Length - 1] + 1, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                                GameManager.Instance.addSpecificScorePoints(playersPosition[playersPosition.Length - 2] + 1, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                                GameManager.Instance.addSpecificScorePoints(playersPosition[playersPosition.Length - 3] + 1, GameManager.Instance.scoreValuesManagerScript.PointsSecondPlace);
                                break;
                            case EqualityCase.SecondThird:
                                GameManager.Instance.addSpecificScorePoints(playersPosition[playersPosition.Length - 1] + 1, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                                GameManager.Instance.addSpecificScorePoints(playersPosition[playersPosition.Length - 2] + 1, GameManager.Instance.scoreValuesManagerScript.PointsSecondPlace);
                                GameManager.Instance.addSpecificScorePoints(playersPosition[playersPosition.Length - 3] + 1, GameManager.Instance.scoreValuesManagerScript.PointsSecondPlace);
                                break;
                        }
                        break;
                    case 2:
                        switch (equalityCase)
                        {
                            case EqualityCase.None:
                                GameManager.Instance.addSpecificScorePoints(playersPosition[playersPosition.Length - 1] + 1, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                                GameManager.Instance.addSpecificScorePoints(playersPosition[playersPosition.Length - 2] + 1, GameManager.Instance.scoreValuesManagerScript.PointsSecondPlace);
                                break;
                            case EqualityCase.AllEqualDifferentZero:
                                GameManager.Instance.addSpecificScorePoints(playersPosition[playersPosition.Length - 1] + 1, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                                GameManager.Instance.addSpecificScorePoints(playersPosition[playersPosition.Length - 2] + 1, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                                break;
                        }
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
                    switch (equalityCase)
                    {
                        case EqualityCase.None:
                            if (playersTeam[i] == playersTeam[playersPosition[playersPosition.Length - 1]]) GameManager.Instance.addSpecificScorePoints(i + 1, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                            else GameManager.Instance.addSpecificScorePoints(i + 1, GameManager.Instance.scoreValuesManagerScript.PointsSecondPlace);
                            break;
                        case EqualityCase.SecondThirdFourth:
                            if (playersTeam[i] == playersTeam[playersPosition[playersPosition.Length - 1]]) GameManager.Instance.addSpecificScorePoints(i + 1, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                            else GameManager.Instance.addSpecificScorePoints(i + 1, GameManager.Instance.scoreValuesManagerScript.PointsSecondPlace);
                            break;
                        case EqualityCase.FirstSecond:
                            if (playersTeam[playersPosition[playersPosition.Length - 1]] == playersTeam[playersPosition[playersPosition.Length - 2]])
                            {
                                if (playersTeam[i] == playersTeam[playersPosition[playersPosition.Length - 1]]) GameManager.Instance.addSpecificScorePoints(i + 1, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                                else GameManager.Instance.addSpecificScorePoints(i + 1, GameManager.Instance.scoreValuesManagerScript.PointsSecondPlace);
                            }
                            else
                            {
                                if (playersTeam[i] == playersTeam[playersPosition[playersPosition.Length - 1]]) GameManager.Instance.addSpecificScorePoints(i + 1, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                                else GameManager.Instance.addSpecificScorePoints(i + 1, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                            }
                            break;
                        case EqualityCase.SecondThird:
                            if (playersTeam[i] == playersTeam[playersPosition[playersPosition.Length - 1]]) GameManager.Instance.addSpecificScorePoints(i + 1, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                            else GameManager.Instance.addSpecificScorePoints(i + 1, GameManager.Instance.scoreValuesManagerScript.PointsSecondPlace);
                            break;
                        case EqualityCase.ThirdFourth:
                            if (playersTeam[i] == playersTeam[playersPosition[playersPosition.Length - 1]]) GameManager.Instance.addSpecificScorePoints(i + 1, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                            else GameManager.Instance.addSpecificScorePoints(i + 1, GameManager.Instance.scoreValuesManagerScript.PointsSecondPlace);
                            break;
                        case EqualityCase.AllEqualDifferentZero:
                            if (playersTeam[i] == playersTeam[playersPosition[playersPosition.Length - 1]]) GameManager.Instance.addSpecificScorePoints(i + 1, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                            else GameManager.Instance.addSpecificScorePoints(i + 1, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                            break;
                        case EqualityCase.FirstSecondAndThirdFourth:
                            if (playersTeam[playersPosition[playersPosition.Length - 1]] == playersTeam[playersPosition[playersPosition.Length - 2]])
                            {
                                if (playersTeam[i] == playersTeam[playersPosition[playersPosition.Length - 1]]) GameManager.Instance.addSpecificScorePoints(i + 1, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                                else GameManager.Instance.addSpecificScorePoints(i + 1, GameManager.Instance.scoreValuesManagerScript.PointsSecondPlace);
                            }
                            else
                            {
                                if (playersTeam[i] == playersTeam[playersPosition[playersPosition.Length - 1]]) GameManager.Instance.addSpecificScorePoints(i + 1, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                                else GameManager.Instance.addSpecificScorePoints(i + 1, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                            }
                            break;
                        case EqualityCase.FirstSecondThird:
                            if (playersTeam[playersPosition[playersPosition.Length - 1]] == playersTeam[playersPosition[playersPosition.Length - 2]] && playersTeam[playersPosition[playersPosition.Length - 1]] == playersTeam[playersPosition[playersPosition.Length - 3]])
                            {
                                if (playersTeam[i] == playersTeam[playersPosition[playersPosition.Length - 1]]) GameManager.Instance.addSpecificScorePoints(i + 1, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                                else GameManager.Instance.addSpecificScorePoints(i + 1, GameManager.Instance.scoreValuesManagerScript.PointsSecondPlace);
                            }
                            else
                            {
                                if (playersTeam[i] == playersTeam[playersPosition[playersPosition.Length - 1]]) GameManager.Instance.addSpecificScorePoints(i + 1, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                                else GameManager.Instance.addSpecificScorePoints(i + 1, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                            }
                            break;
                    }
                }
                break;
            case (int)GameManager.TeamCompo.TwoVSTwo:
                for (int i = 0; i < players.Length; i++)
                {
                    switch (equalityCase)
                    {
                        case EqualityCase.None:
                            if (playersTeam[i] == playersTeam[playersPosition[playersPosition.Length - 1]]) GameManager.Instance.addSpecificScorePoints(i + 1, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                            else GameManager.Instance.addSpecificScorePoints(i + 1, GameManager.Instance.scoreValuesManagerScript.PointsSecondPlace);
                            break;
                        case EqualityCase.SecondThirdFourth:
                            if (playersTeam[i] == playersTeam[playersPosition[playersPosition.Length - 1]]) GameManager.Instance.addSpecificScorePoints(i + 1, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                            else GameManager.Instance.addSpecificScorePoints(i + 1, GameManager.Instance.scoreValuesManagerScript.PointsSecondPlace);
                            break;
                        case EqualityCase.FirstSecond:
                            if (playersTeam[i] == playersTeam[playersPosition[playersPosition.Length - 1]]) GameManager.Instance.addSpecificScorePoints(i + 1, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                            else GameManager.Instance.addSpecificScorePoints(i + 1, GameManager.Instance.scoreValuesManagerScript.PointsSecondPlace);
                            break;
                        case EqualityCase.SecondThird:
                            if (playersTeam[i] == playersTeam[playersPosition[playersPosition.Length - 1]]) GameManager.Instance.addSpecificScorePoints(i + 1, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                            else GameManager.Instance.addSpecificScorePoints(i + 1, GameManager.Instance.scoreValuesManagerScript.PointsSecondPlace);
                            break;
                        case EqualityCase.ThirdFourth:
                            if (playersTeam[i] == playersTeam[playersPosition[playersPosition.Length - 1]]) GameManager.Instance.addSpecificScorePoints(i + 1, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                            else GameManager.Instance.addSpecificScorePoints(i + 1, GameManager.Instance.scoreValuesManagerScript.PointsSecondPlace);
                            break;
                        case EqualityCase.AllEqualDifferentZero:
                            if (playersTeam[i] == playersTeam[playersPosition[playersPosition.Length - 1]]) GameManager.Instance.addSpecificScorePoints(i + 1, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                            else GameManager.Instance.addSpecificScorePoints(i + 1, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                            break;
                        case EqualityCase.FirstSecondAndThirdFourth:
                            if (playersTeam[playersPosition[playersPosition.Length - 1]] == playersTeam[playersPosition[playersPosition.Length - 2]])
                            {
                                if (playersTeam[i] == playersTeam[playersPosition[playersPosition.Length - 1]]) GameManager.Instance.addSpecificScorePoints(i + 1, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                                else GameManager.Instance.addSpecificScorePoints(i + 1, GameManager.Instance.scoreValuesManagerScript.PointsSecondPlace);
                            }
                            else
                            {
                                if (playersTeam[i] == playersTeam[playersPosition[playersPosition.Length - 1]]) GameManager.Instance.addSpecificScorePoints(i + 1, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                                else GameManager.Instance.addSpecificScorePoints(i + 1, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                            }
                            break;
                        case EqualityCase.FirstSecondThird:
                            if (playersTeam[i] == playersTeam[playersPosition[playersPosition.Length - 1]]) GameManager.Instance.addSpecificScorePoints(i + 1, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                            else GameManager.Instance.addSpecificScorePoints(i + 1, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                            break;
                    }
                }
                break;
        }

        playOneTime = true;
    }

    private void CheckIfEquality()
    {
        //Check if no players have points
        for (int i = 0; i < scorePlayers.Length; i++)
        {
            if (scorePlayers[i] != 0)
            {
                allScoreZero = false;
                break;
            }
        }

        //Check if equality between players
        for (int i = 0; i < finalScores.Length; i++)
        {
            if (finalScores.Length == 4)
            {
                if (i == 3)
                {
                    if (finalScores[i] == finalScores[i - 1])
                    {
                        //equality between 1st & 2nd
                        equalityCase = EqualityCase.FirstSecond;
                        //Debug.Log("equality between 1st & 2nd");
                    }
                }
                else if (i == 2)
                {
                    if (finalScores[i] == finalScores[i - 1])
                    {
                        //equality between 2st & 3rd
                        equalityCase = EqualityCase.SecondThird;
                        //Debug.Log("equality between 2st & 3rd");
                    }
                }
                else if (i == 1)
                {
                    if (finalScores[i] == finalScores[i + 2] && finalScores[i] == finalScores[i - 1])
                    {
                        if (allScoreZero)
                        {
                            //equality between all players
                            equalityCase = EqualityCase.AllEqual;
                            //Debug.Log("equality between all players");
                            break;
                        }
                        else
                        {
                            //equality between all players different from 0
                            equalityCase = EqualityCase.AllEqualDifferentZero;
                            //Debug.Log("equality between all players different from 0");
                            break;
                        }
                    }
                    else if (finalScores[i] == finalScores[i + 2])
                    {
                        //equality between 1st & 2nd & 3rd
                        equalityCase = EqualityCase.FirstSecondThird;
                        //Debug.Log("equality between 1st & 2nd & 3rd");
                        break;
                    }
                    else if (finalScores[i] == finalScores[i - 1] && finalScores[i] == finalScores[i + 1])
                    {
                        //equality between 2st & 3rd & 4th
                        equalityCase = EqualityCase.SecondThirdFourth;
                        //Debug.Log("equality between 2st & 3rd & 4th");
                        break;
                    }
                    else if (finalScores[i] == finalScores[i - 1])
                    {
                        if (finalScores[i + 2] == finalScores[(i + 2) - 1])
                        {
                            //equality between 1st & 2nd and 3rd & 4th
                            equalityCase = EqualityCase.FirstSecondAndThirdFourth;
                            //Debug.Log("equality between 1st & 2nd and 3rd & 4th");
                            break;
                        }
                        else
                        {
                            //equality between 3rd & 4th
                            equalityCase = EqualityCase.ThirdFourth;
                            //Debug.Log("equality between 3rd & 4th");
                        }
                    }
                }
            }
            else if (finalScores.Length == 3)
            {
                if (i == 1)
                {
                    if (finalScores[i] == finalScores[i - 1] && finalScores[i] == finalScores[i + 1])
                    {
                        if (allScoreZero)
                        {
                            //equality between all players
                            equalityCase = EqualityCase.AllEqual;
                            //Debug.Log("equality between all players");
                            break;
                        }
                        else
                        {
                            //equality between all players different from 0
                            equalityCase = EqualityCase.AllEqualDifferentZero;
                            //Debug.Log("equality between all players different from 0");
                            break;
                        }
                    }
                    else if (finalScores[i] == finalScores[i + 1])
                    {
                        //equality between 1st & 2nd
                        equalityCase = EqualityCase.FirstSecond;
                        //Debug.Log("equality between 1st & 2nd");
                    }
                    else if (finalScores[i] == finalScores[i - 1])
                    {
                        //equality between 2st & 3rd
                        equalityCase = EqualityCase.SecondThird;
                        //Debug.Log("equality between 2st & 3rd");
                    }
                }
            }
            else if (finalScores.Length == 2)
            {
                if (i == 1)
                {
                    if (finalScores[i] == finalScores[i - 1])
                    {
                        if (allScoreZero)
                        {
                            //equality between all players
                            equalityCase = EqualityCase.AllEqual;
                            //Debug.Log("equality between all players");
                            break;
                        }
                        else
                        {
                            //equality between all players different from 0
                            equalityCase = EqualityCase.AllEqualDifferentZero;
                            //Debug.Log("equality between all players different from 0");
                            break;
                        }
                    }
                }
            }
        }
    }

    private void SpawnPlayerRandomly()
    {
        //randomNumbers = GenerateRandomNumbers(4, 0, 4);

        for (int i = 0; i < players.Length; i++)
        {
            players[i].transform.position = tpPoints[i].position;
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

    public enum EqualityCase
    {
        None,
        AllEqual,
        AllEqualDifferentZero,
        FirstSecondThird,
        SecondThirdFourth,
        FirstSecond,
        ThirdFourth,
        SecondThird,
        FirstSecondAndThirdFourth
    }
}
