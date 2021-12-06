using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WhackAMoleManager : MonoBehaviour
{
    private GameObject[] playersUnsorted;
    public GameObject[] players;

    public List<Transform> tpPoints = new List<Transform>();
    private List<int> randomNumbers = new List<int>();

    [Space]
    public Transform TpFolder;
    public Transform molesFolder;

    [Header("Settings")]
    public int moleAtStart = 0;
    //public int destroyedMoleToRespawn = 0;
    //public int numberMoleToRespawn = 0;
    [HideInInspector] public int MoleInScene = 0;
    //[HideInInspector] public int moleDestroyed;

    [Header("Mole")]
    public GameObject molePrefab;
    public float moleTimeStay;

    [Header("Score")]
    public Score scoreScript;
    public Timer timerScript;
    public float[] scorePlayers;
    private float[] finalScores;
    private int[] playersPosition;

    public List<Transform> allTpPoints;

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

        for (int i = 0; i < TpFolder.childCount; i++)
        {
            if (TpFolder.GetChild(i).GetComponent<CheckPlayerIsClose>() != null)
            {
                allTpPoints.Add(TpFolder.GetChild(i));
            }
        }

        for (int i = 0; i < moleAtStart; i++)
        {
            spawnMole();
        }

        SpawnPlayerRandomly();
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
        }*/

        if (MoleInScene <= moleAtStart)
        {
            for (int i = MoleInScene; i < moleAtStart; i++)
            {
                spawnMole();
            }
        }

        if (timerScript.miniGameTimer <= 0)
        {
            SortPlayers();
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

        if (!allTpPoints[randomNumberChosen].GetComponent<CheckPlayerIsClose>().playerIsClose && !allTpPoints[randomNumberChosen].GetComponent<CheckPlayerIsClose>().alreadyMole)
        {
            allTpPoints[randomNumberChosen].GetComponent<CheckPlayerIsClose>().alreadyMole = true;
            GameObject moleInstance = Instantiate(molePrefab, allTpPoints[randomNumberChosen].position, Quaternion.identity, molesFolder);
            moleInstance.GetComponent<Mole>().currentTpScript = allTpPoints[randomNumberChosen].GetComponent<CheckPlayerIsClose>();
            moleInstance.GetComponent<Mole>().moleTimeStay = moleTimeStay;
            MoleInScene++;
        }
        else
        {
            spawnMole();
        } 
    }

    private void SortPlayers()
    {
        for (int i = 0; i < finalScores.Length; i++)
        {
            finalScores[i] = scorePlayers[i];
        }

        System.Array.Sort(finalScores, playersPosition);

        switch (playersPosition.Length)
        {
            case 4:
                GameManager.Instance.addSpecificScore(playersPosition[playersPosition.Length - 1] + 1, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                GameManager.Instance.addSpecificScore(playersPosition[playersPosition.Length - 2] + 1, GameManager.Instance.scoreValuesManagerScript.PointsSecondPlace);
                GameManager.Instance.addSpecificScore(playersPosition[playersPosition.Length - 3] + 1, GameManager.Instance.scoreValuesManagerScript.PointsThirdPlace);
                GameManager.Instance.addSpecificScore(playersPosition[playersPosition.Length - 4] + 1, GameManager.Instance.scoreValuesManagerScript.PointsFourthPlace);
                break;
            case 3:
                GameManager.Instance.addSpecificScore(playersPosition[playersPosition.Length - 1] + 1, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                GameManager.Instance.addSpecificScore(playersPosition[playersPosition.Length - 2] + 1, GameManager.Instance.scoreValuesManagerScript.PointsSecondPlace);
                GameManager.Instance.addSpecificScore(playersPosition[playersPosition.Length - 3] + 1, GameManager.Instance.scoreValuesManagerScript.PointsThirdPlace);
                break;
            case 2:
                GameManager.Instance.addSpecificScore(playersPosition[playersPosition.Length - 1] + 1, GameManager.Instance.scoreValuesManagerScript.PointsFirstPlace);
                GameManager.Instance.addSpecificScore(playersPosition[playersPosition.Length - 2] + 1, GameManager.Instance.scoreValuesManagerScript.PointsSecondPlace);
                break;
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
