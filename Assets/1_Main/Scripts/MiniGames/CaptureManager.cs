using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class CaptureManager : MonoBehaviour
{
    private GameObject[] playersUnsorted;
    public GameObject[] players;

    [HideInInspector] public float[] scorePlayers;
    private float[] finalScores;
    private int[] playersPosition;

    private int scorePlayer0 = 0;
    private int scorePlayer1 = 0;
    private int scorePlayer2 = 0;
    private int scorePlayer3 = 0;
    private int pointCounterP1 = 1;
    private int pointCounterP2 = 1;
    private int pointCounterP3 = 1;
    private int pointCounterP4 = 1;
    private Zone zoneScript;
    public float timePastInZone = 0;
    public float[] timePastInZonePlayer;

    [Space]
    public float timeToScore = 1;
    public int pointsEarned1V3Alone = 2;
    private CaptureSound captureSoundScript;
    public Score scoreScriptTeam;
    public Score scoreScript;
    public Timer timerScript;
    private bool zoneSound = false;

    [Header ("Score")]
    public GameObject floatingPoint;
    public Vector2 spawnPointOffset;
    private GameObject instFloatingPointP1;
    private GameObject instFloatingPointP2;
    private GameObject instFloatingPointP3;
    private GameObject instFloatingPointP4;
    public Color player0Color;
    public Color player1Color;
    public Color player2Color;
    public Color player3Color;

    public List<Transform> tpPoints = new List<Transform>();
    private List<int> randomNumbers = new List<int>();

    private bool playOneTime = false;

    private bool allScoreZero = true;

    //animator
    private Animator lightAnimator;

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
        playersUnsorted = GameObject.FindGameObjectsWithTag("Player");
        players = playersUnsorted.OrderBy(go => go.name).ToArray();

        scorePlayers = new float[players.Length];
        finalScores = new float[players.Length];
        playersPosition = new int[players.Length];
        timePastInZonePlayer = new float[players.Length];
        for (int i = 0; i < players.Length; i++)
        {
            playersPosition[i] = i;
        }

        for (int i = 0; i < timePastInZonePlayer.Length; i++)
        {
            timePastInZonePlayer[i] = timeToScore;
        }

        zoneScript = GetComponentInChildren<Zone>();
        captureSoundScript = GetComponentInChildren<CaptureSound>();

        SpawnPlayerRandomly();
        //GameManager.Instance.focusPlayersScript.SetGameTitle("Zone");

        playersTeam = GameManager.Instance.playersTeam;
        
        if (GameManager.Instance.getTeamCompo() == 1)
        {
            OneVSThreeEnable = true;
            playerAlone1v3 = System.Array.IndexOf(playersTeam, 0);
        }

        if (GetComponent<Animator>() != null)
        {
            lightAnimator = GetComponent<Animator>();
            lightAnimator.enabled = false;
        }
    }

    private void Update()
    {
        DisplayUITeam();

        if (timerScript.miniGameTimer > 0)
        {
            IncrementPlayerScore();
        }

        if (timerScript.isTimerStarted && lightAnimator != null)
        {
            if (!lightAnimator.enabled)
            {
                //activate light movement
                lightAnimator.enabled = true;
            }
        }

        if (timerScript.miniGameTimer <= 0 && !playOneTime)
        {
            EqualizeScoreIfTeam();
            SortPlayers();

            if (lightAnimator != null)
            {
                //deactivate light movement
                lightAnimator.enabled = false;
            }

            ResetColor();

            playOneTime = true;
        }

        if (timerScript.miniGameTimer > 0 && timerScript.isTimerStarted)
        {
            MultiplePlayersCaptureZone();
        }
        else
        {
            if (zoneSound)
            {
                zoneSound = false;
                captureSoundScript.PlayerOutZone();
            }
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

    private void MultiplePlayersCaptureZone()
    {
        if (zoneScript.counterPlayerinZone > 0)
        {
            if (!zoneSound)
            {
                zoneSound = true;
                captureSoundScript.PlayerCapturing();
            }
        }
        else
        {
            if (zoneSound)
            {
                zoneSound = false;
                captureSoundScript.PlayerOutZone();
            }
            
        }

        //Player 1 score
        if (zoneScript.player0IsInZone)
        {
            if (timePastInZonePlayer[0] >= timeToScore)
            {
                if (OneVSThreeEnable && playerAlone1v3 == 0)
                {
                    scorePlayer0 += pointsEarned1V3Alone;
                    scoreScript.AddScore(pointsEarned1V3Alone, 0, 0, 0);
                    pointCounterP1 += pointsEarned1V3Alone;
                    MergeScoreTeam(0, pointsEarned1V3Alone);
                    
                }
                else
                {
                    scorePlayer0++;
                    scoreScript.AddScore(1, 0, 0, 0);
                    pointCounterP1++;
                    MergeScoreTeam(0, 1);
                }

                timePastInZonePlayer[0] = 0;
                SpawnFloatingText(instFloatingPointP1, player0Color, players[0], pointCounterP1);
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
                Destroy(instFloatingPointP1);
            }
        }

        //Player 2 score
        if (zoneScript.player1IsInZone)
        {
            if (timePastInZonePlayer[1] >= timeToScore)
            {
                if (OneVSThreeEnable && playerAlone1v3 == 1)
                {
                    scorePlayer1 += pointsEarned1V3Alone;
                    scoreScript.AddScore(0, pointsEarned1V3Alone, 0, 0);
                    pointCounterP2 += pointsEarned1V3Alone;
                    MergeScoreTeam(1, pointsEarned1V3Alone);

                }
                else
                {
                    scorePlayer1++;
                    scoreScript.AddScore(0, 1, 0, 0);
                    pointCounterP2++;
                    MergeScoreTeam(1, 1);
                }

                timePastInZonePlayer[1] = 0;
                SpawnFloatingText(instFloatingPointP2, player1Color, players[1], pointCounterP2);
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
                Destroy(instFloatingPointP2);
            } 
        }

        //Player 3 score
        if (zoneScript.player2IsInZone)
        {
            if (timePastInZonePlayer[2] >= timeToScore)
            {
                if (OneVSThreeEnable && playerAlone1v3 == 2)
                {
                    scorePlayer2 += pointsEarned1V3Alone;
                    scoreScript.AddScore(0, 0, pointsEarned1V3Alone, 0);
                    pointCounterP3 += pointsEarned1V3Alone;
                    MergeScoreTeam(2, pointsEarned1V3Alone);

                }
                else
                {
                    scorePlayer2++;
                    scoreScript.AddScore(0, 0, 1, 0);
                    pointCounterP3++;
                    MergeScoreTeam(2, 1);
                }

                timePastInZonePlayer[2] = 0;
                SpawnFloatingText(instFloatingPointP3, player2Color, players[2], pointCounterP3);
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
                Destroy(instFloatingPointP3);
            }
        }

        //Player 4 score
        if (zoneScript.player3IsInZone)
        {
            if (timePastInZonePlayer[3] >= timeToScore)
            {
                if (OneVSThreeEnable && playerAlone1v3 == 3)
                {
                    scorePlayer3 += pointsEarned1V3Alone;
                    scoreScript.AddScore(0, 0, 0, pointsEarned1V3Alone);
                    pointCounterP4 += pointsEarned1V3Alone;
                    MergeScoreTeam(3, pointsEarned1V3Alone);

                }
                else
                {
                    scorePlayer3++;
                    scoreScript.AddScore(0, 0, 0, 1);
                    pointCounterP4++;
                    MergeScoreTeam(3, 1);
                }

                timePastInZonePlayer[3] = 0;
                SpawnFloatingText(instFloatingPointP4, player3Color, players[3], pointCounterP4);
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
                Destroy(instFloatingPointP4);
            }
        }
    }

    private void SpawnFloatingText(GameObject instFloatingText, Color playerColor, GameObject player, int counter)
    {
        if (instFloatingText != null) Destroy(instFloatingText);
        instFloatingText = Instantiate(floatingPoint, new Vector2(player.transform.position.x + spawnPointOffset.x, player.transform.position.y + spawnPointOffset.y), Quaternion.identity, player.transform);
        instFloatingText.transform.GetChild(0).GetComponent<TextMeshPro>().text = "+ " + counter.ToString();
        instFloatingText.transform.GetChild(0).GetComponent<TextMeshPro>().color = playerColor;
    }

    private void OnePlayerCaptureZone()
    {
        if (zoneScript.counterPlayerinZone >= 2)
        {
            timePastInZone = 0;
            captureSoundScript.PlayerOutZone();
            zoneSound = false;

        }
        else
        {
            if (zoneScript.player0IsInZone == true)
            {
                timePastInZone += Time.deltaTime;


                if (!zoneSound)
                {
                    zoneSound = true;
                    captureSoundScript.PlayerCapturing();
                }


                if (timePastInZone >= timeToScore)
                {
                    scorePlayer0++;
                    timePastInZone = 0;
                    scoreScript.AddScore(1, 0, 0, 0);
                }
            }

            if (zoneScript.player1IsInZone == true)
            {
                timePastInZone += Time.deltaTime;


                if (!zoneSound)
                {
                    zoneSound = true;
                    captureSoundScript.PlayerCapturing();
                }

                if (timePastInZone >= timeToScore)
                {
                    scorePlayer1++;
                    timePastInZone = 0;
                    scoreScript.AddScore(0, 1, 0, 0);
                }
            }

            if (zoneScript.player2IsInZone == true)
            {
                timePastInZone += Time.deltaTime;


                if (!zoneSound)
                {
                    zoneSound = true;
                    captureSoundScript.PlayerCapturing();
                }

                if (timePastInZone >= timeToScore)
                {
                    scorePlayer2++;
                    timePastInZone = 0;
                    scoreScript.AddScore(0, 0, 1, 0);
                }
            }

            if (zoneScript.player3IsInZone == true)
            {
                timePastInZone += Time.deltaTime;


                if (!zoneSound)
                {
                    zoneSound = true;
                    captureSoundScript.PlayerCapturing();
                }

                if (timePastInZone >= timeToScore)
                {
                    scorePlayer3++;
                    timePastInZone = 0;
                    scoreScript.AddScore(0, 0, 0, 1);
                }
            }
        }

        if (zoneScript.counterPlayerinZone < 1)
        {
            timePastInZone = 0;
            captureSoundScript.PlayerOutZone();
            zoneSound = false;
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

    private void IncrementPlayerScore()
    {
        if (scorePlayers.Length == 2)
        {
            scorePlayers[0] = scoreScript.scorePlayer0;
            scorePlayers[1] = scoreScript.scorePlayer1;
        }
        else if (scorePlayers.Length == 3)
        {
            scorePlayers[0] = scoreScript.scorePlayer0;
            scorePlayers[1] = scoreScript.scorePlayer1;
            scorePlayers[2] = scoreScript.scorePlayer2;
        }
        else if (scorePlayers.Length == 4)
        {
            scorePlayers[0] = scoreScript.scorePlayer0;
            scorePlayers[1] = scoreScript.scorePlayer1;
            scorePlayers[2] = scoreScript.scorePlayer2;
            scorePlayers[3] = scoreScript.scorePlayer3;
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

    private void SortPlayerWithOneWinner()
    {
        int maxVal = 0;
        int joueurValMax = 0;
        if (scorePlayer0 > maxVal)
        {
            maxVal = scorePlayer0;
            joueurValMax = 0;
        }
        if (scorePlayer1 > maxVal)
        {
            maxVal = scorePlayer1;
            joueurValMax = 1;
        }
        if (scorePlayer2 > maxVal)
        {
            maxVal = scorePlayer2;
            joueurValMax = 2;
        }
        if (scorePlayer3 > maxVal)
        {
            maxVal = scorePlayer3;
            joueurValMax = 3;
        }

        switch (joueurValMax)
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