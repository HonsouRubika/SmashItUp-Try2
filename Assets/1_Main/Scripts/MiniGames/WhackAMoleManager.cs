using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WhackAMoleManager : MonoBehaviour
{
    private GameObject[] playersUnsorted;
    public GameObject[] players;

    [HideInInspector] public float[] scorePlayers;
    private float[] finalScores;
    private int[] playersPosition;

    public List<Transform> tpPoints = new List<Transform>();
    private List<int> randomNumbers = new List<int>();

    [Space]
    public Transform TpFolder;
    public Transform molesFolder;

    [Header("Settings")]
    public int moleAtStart = 2;
    //public int destroyedMoleToRespawn = 0;
    //public int numberMoleToRespawn = 0;
    [HideInInspector] public int MoleInScene = 0;
    //[HideInInspector] public int moleDestroyed;

    [Header("Mole")]
    public GameObject molePrefab;
    public float moleTimeStay = 2;

    [Header("UI")]
    public Score scoreScript;
    public Timer timerScript;

    public List<Transform> allTpPoints;

    public List<int> previousRandomNumber;
    //public List<float> timesMoleSpawned;
    public float delaySpawnMoleTime = 2;
    //public int[] previousRandomNumberVector;

    private bool playOneTime = false;

    private bool allScoreZero = true;

    //team compo
    public int[] playersTeam;
    [HideInInspector] public int playerAlone1v3 = 0;
    [HideInInspector] public bool OneVSThreeEnable = false;

    //Equality
    public EqualityCase equalityCase = EqualityCase.None;


    public TaupeSound TaupeSoundScript;

    //Merge score team
    private float scoreTeam1;
    private float scoreTeam2;

    private void Start()
    {
        //init var
        playersUnsorted = GameObject.FindGameObjectsWithTag("Player");
        players = playersUnsorted.OrderBy(go => go.name).ToArray();

        scorePlayers = new float[players.Length];
        finalScores = new float[players.Length];
        playersPosition = new int[players.Length];
        for (int i = 0; i < players.Length; i++)
        {
            playersPosition[i] = i;
        }

        for (int i = 0; i < TpFolder.childCount; i++)
        {
            if (TpFolder.GetChild(i).GetComponent<CheckPlayerIsClose>() != null)
            {
                allTpPoints.Add(TpFolder.GetChild(i));
            }
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
        /*if (moleDestroyed >= destroyedMoleToRespawn)
        {
            for (int i = 0; i < numberMoleToRespawn; i++)
            {
                spawnMole();
            }

            moleDestroyed = 0;
            
            TaupeSoundScript.TaupeisHit();
        }*/

        if (timerScript.isTimerStarted)
        {
            if (MoleInScene < moleAtStart)
            {
                for (int i = MoleInScene; i < moleAtStart; i++)
                {
                    spawnMole();
                }
            }
        }

        if (timerScript.miniGameTimer <= 0 && !playOneTime)
        {
            EqualizeScoreIfTeam();
            SortPlayers();

            ResetColor();

            playOneTime = true;
        }
    }

    public void MergeScoreTeam(int player, float score)
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
    private int ChooseRandomNumber(int start, int end)
    {
        int randomNumber = Random.Range(start, end);

        return randomNumber;
    }

    private void spawnMole()
    {
        int randomNumberChosen = ChooseRandomNumber(0, allTpPoints.Count);

        //test
        //if (allTpPoints[randomNumberChosen].GetComponent<CheckPlayerIsClose>() == null) Debug.Log("c'est null");

        
        if (!previousRandomNumber.Contains(randomNumberChosen) && !allTpPoints[randomNumberChosen].GetComponent<CheckPlayerIsClose>().alreadyMole 
            && allTpPoints[randomNumberChosen].GetComponent<CheckPlayerIsClose>().moleSpawnedTime + delaySpawnMoleTime < Time.time) //consition de spawn => évite un spawnkill non-intentionnel
        {
            previousRandomNumber.Add(randomNumberChosen);

            allTpPoints[randomNumberChosen].GetComponent<CheckPlayerIsClose>().alreadyMole = true;
            GameObject moleInstance = Instantiate(molePrefab, allTpPoints[randomNumberChosen].position, Quaternion.identity, molesFolder);
            Mole moleScript = moleInstance.GetComponent<Mole>();
            moleScript.currentTpScript = allTpPoints[randomNumberChosen].GetComponent<CheckPlayerIsClose>();
            moleScript.moleTimeStay = moleTimeStay;
            moleScript.moleID = randomNumberChosen;
            moleScript.whackAMoleScript = this;
            MoleInScene++;
            TaupeSoundScript.TaupeSpawning();
        }
        /*
        else
        {
            //no spawner actually available
            //spawnMole();
        }
        */

    }

    public void despawnMole(int moleID)
    {
        for (int i = 0; i< MoleInScene; i++)
        {
            if(previousRandomNumber[i] == moleID)
            {
                previousRandomNumber.RemoveAt(i);
                allTpPoints[moleID].GetComponent<CheckPlayerIsClose>().moleSpawnedTime = Time.time + moleTimeStay;
                MoleInScene--;
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
}
