using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreValuesManager : MonoBehaviour
{
    //Points to add depending to Player Position
    [HideInInspector] public int PointsFirstPlace;
    [HideInInspector] public int PointsSecondPlace;
    [HideInInspector] public int PointsThirdPlace;
    [HideInInspector] public int PointsFourthPlace;

    [Header("4 Players case")]
    [Range(0, 100)] public int FourPlayersPointsFirstPlace;
    [Range(0, 100)] public int FourPlayersPointsSecondPlace;
    [Range(0, 100)] public int FourPlayersPointsThirdPlace;
    [Range(0, 100)] public int FourPlayersPointsFourthPlace;

    [Header("3 Players case")]
    [Range(0, 100)] public int ThreePlayersPointsFirstPlace;
    [Range(0, 100)] public int ThreePlayersPointsSecondPlace;
    [Range(0, 100)] public int ThreePlayersPointsThirdPlace;

    [Header("2 Players case")]
    [Range(0, 100)] public int TwoPlayersPointsFirstPlace;
    [Range(0, 100)] public int TwoPlayersSecondPlace;

    [Space]
    public GameObject[] players;

    private void Update()
    {
        players = GameObject.FindGameObjectsWithTag("Player");

        PointsDependingPlayersNumber();
    }

    public void PointsDependingPlayersNumber()
    {
        switch (players.Length)
        {
            case 4:
                PointsFirstPlace = FourPlayersPointsFirstPlace;
                PointsSecondPlace = FourPlayersPointsSecondPlace;
                PointsThirdPlace = FourPlayersPointsThirdPlace;
                PointsFourthPlace = FourPlayersPointsFourthPlace;
                break;
            case 3:
                PointsFirstPlace = ThreePlayersPointsFirstPlace;
                PointsSecondPlace = ThreePlayersPointsSecondPlace;
                PointsThirdPlace = ThreePlayersPointsThirdPlace;
                break;
            case 2:
                PointsFirstPlace = TwoPlayersPointsFirstPlace;
                PointsSecondPlace = TwoPlayersSecondPlace;
                break;
        }
    }
}
