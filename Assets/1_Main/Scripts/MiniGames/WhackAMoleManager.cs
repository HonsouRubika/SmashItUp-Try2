using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhackAMoleManager : MonoBehaviour
{
    public Transform TpFolder;
    public Transform molesFolder;

    [Header("Mole")]
    public GameObject molePrefab;

    [Header("Score")]
    public Score scoreScript;

    public List<Transform> allTpPoints;

    private void Start()
    {
        for (int i = 0; i < TpFolder.childCount; i++)
        {
            allTpPoints.Add(TpFolder.GetChild(i));
        }

        spawnMole();
    }

    private int ChooseRandomNumber(int start, int end)
    {
        int randomNumber = Random.Range(start, end);

        return randomNumber;
    }

    private void spawnMole()
    {
        int randomNumberChosen = ChooseRandomNumber(0, allTpPoints.Count);

        if (!allTpPoints[randomNumberChosen].GetComponent<CheckPlayerIsClose>().playerIsClose)
        {
            Instantiate(molePrefab, allTpPoints[randomNumberChosen].position, Quaternion.identity, molesFolder);
        }
        else
        {
            spawnMole();
        } 
    }
}
