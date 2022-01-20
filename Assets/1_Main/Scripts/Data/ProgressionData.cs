using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProgressionData
{
    private int nbGameFinished;

    public ProgressionData(GameManager gameManager)
    {
        nbGameFinished = gameManager.nbGameFinished;
    }

    public void SetNbGameFinished(int nb)
    {
        nbGameFinished = nb;
    }

    public int GetNbGameFinished()
    {
        return nbGameFinished;
    }
}
