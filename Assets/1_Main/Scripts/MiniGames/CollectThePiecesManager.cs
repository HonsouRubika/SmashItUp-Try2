using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CollectThePiecesManager : MonoBehaviour
{
    private GameObject[] playersUnsorted;
    public GameObject[] players;

    private float[] scorePlayers;
    private float[] finalScores;
    private int[] playersPosition;

    public int piecesNumber = 0;

    [Space]
    public Score scoreScript;
    public Timer timerScript;

    public List<Transform> tpPoints = new List<Transform>();
    private List<int> randomNumbers = new List<int>();

    private bool playOneTime = false;

    private bool allScoreZero = true;

    //team compo
    public int[] playersTeam;
    [HideInInspector] public int playerAlone1v3 = 0;
    [HideInInspector] public bool OneVSThreeEnable = false;

    //Equality
    public EqualityCase equalityCase = EqualityCase.None;

    private void Start()
    {
        playersUnsorted = GameObject.FindGameObjectsWithTag("Player");
        players = playersUnsorted.OrderBy(go => go.name).ToArray();

        scorePlayers = new float[players.Length];
        finalScores = new float[players.Length];
        playersPosition = new int[players.Length];
        for (int i = 0; i < players.Length; i++)
        {
            playersPosition[i] = i;
        }

        SpawnPlayerRandomly();

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
        IncrementPlayerScore();

        if (timerScript.miniGameTimer <= 0 && !playOneTime)
        {
            SortPlayers();

            ResetColor();
            
            playOneTime = true;
        }

        //End the mini-game
        if (piecesNumber <= 0 && !playOneTime)
        {
            GameManager.Instance.Score();
            SortPlayers();

            playOneTime = true;
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
