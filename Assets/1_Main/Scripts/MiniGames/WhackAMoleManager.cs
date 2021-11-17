using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WhackAMoleManager : MonoBehaviour
{
    private GameObject[] playersUnsorted;
    public GameObject[] players;

    [Header("TP Points")]
    public Transform tpPoints0;
    public Transform tpPoints1;
    public Transform tpPoints2;
    public Transform tpPoints3;

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

        /*players[0].transform.position = tpPoints0.position;
        players[1].transform.position = tpPoints1.position;
        players[2].transform.position = tpPoints2.position;
        players[3].transform.position = tpPoints3.position;*/

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

        GameManager.Instance.addSpecificScore(playersPosition[playersPosition.Length - 1] + 1, 10);
        GameManager.Instance.addSpecificScore(playersPosition[playersPosition.Length - 2] + 1, 8);
        GameManager.Instance.addSpecificScore(playersPosition[playersPosition.Length - 3] + 1, 6);
        GameManager.Instance.addSpecificScore(playersPosition[playersPosition.Length - 4] + 1, 4);
    }
}
