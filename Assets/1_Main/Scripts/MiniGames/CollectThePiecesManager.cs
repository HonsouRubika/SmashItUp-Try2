using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CollectThePiecesManager : MonoBehaviour
{
    private GameObject[] playersUnsorted;
    public GameObject[] players;

    public float[] scorePlayers;
    private float[] finalScores;
    private int[] playersPosition;

    public int piecesNumber = 0;

    [Space]
    public Score scoreScript;
    public Timer timerScript;

    public List<Transform> tpPoints = new List<Transform>();
    private List<int> randomNumbers = new List<int>();

    private void Start()
    {
        playersUnsorted = GameObject.FindGameObjectsWithTag("Player");
        players = playersUnsorted.OrderBy(go => go.name).ToArray();

        //SpawnPlayerRandomly();
        GameManager.Instance.focusPlayersScript.SetGameTitle("CollectThePieces");

        scorePlayers = new float[players.Length];
        finalScores = new float[players.Length];
        playersPosition = new int[players.Length];
        for (int i = 0; i < players.Length; i++)
        {
            playersPosition[i] = i;
        }
    }

    private void Update()
    {
        IncrementPlayerScore();

        if (timerScript.miniGameTimer <= 0)
        {
            SortPlayers();
        }

        //End the mini-game
        if (piecesNumber <= 0)
        {
            GameManager.Instance.Score();
            SortPlayers();
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
}