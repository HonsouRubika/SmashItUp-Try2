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
    public int moleAtStart = 2;
    //public int destroyedMoleToRespawn = 0;
    //public int numberMoleToRespawn = 0;
    [HideInInspector] public int MoleInScene = 0;
    //[HideInInspector] public int moleDestroyed;

    [Header("Mole")]
    public GameObject molePrefab;
    public float moleTimeStay = 2;

    [Header("Score")]
    public Score scoreScript;
    public Timer timerScript;
    public float[] scorePlayers;
    private float[] finalScores;
    private int[] playersPosition;

    public List<Transform> allTpPoints;

    public List<int> previousRandomNumber;
    //public int[] previousRandomNumberVector;

    private bool playOneTime = false;

    public TaupeSound TaupeSoundScript;

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

        for (int i = 0; i < moleAtStart; i++)
        {
            spawnMole();

            TaupeSoundScript.TaupeSpawning();

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
            
            TaupeSoundScript.TaupeisHit();
        }*/

        if (MoleInScene < moleAtStart)
        {
            for (int i = MoleInScene; i < moleAtStart; i++)
            {
                spawnMole();
                
            }
        }

        if (timerScript.miniGameTimer <= 0 && !playOneTime)
        {
            SortPlayers();

            playOneTime = true;
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

        
        if (!previousRandomNumber.Contains(randomNumberChosen) && !allTpPoints[randomNumberChosen].GetComponent<CheckPlayerIsClose>().alreadyMole)
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
        }
        else
        {
            Debug.Log("search another place");
            spawnMole();
        }

    }

    public void despawnMole(int moleID)
    {
        allTpPoints[moleID].GetComponent<CheckPlayerIsClose>().alreadyMole = false;
        previousRandomNumber.Remove(moleID);
        MoleInScene--;
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
