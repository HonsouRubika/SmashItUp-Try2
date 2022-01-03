using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRuleBumper : MonoBehaviour
{
    private bool isActive = false; //pour animation
    public RULE bumperRule = RULE.nbManches;

    //range of value
    private int[] rangeNbManches = { 5, 7, 10 };
    private int[] rangeDureeManche = { 30, 45, 60 };
    private int iterator = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        isActive = true; //pour animation
        iterator++;

        //change a rule
        switch (bumperRule)
        {
            case RULE.nbManches:
                if (iterator == rangeNbManches.Length) iterator = 0;
                GameManager.Instance._nbManches = rangeNbManches[iterator];
                break;
            case RULE.dureeManche:
                if (iterator == rangeDureeManche.Length) iterator = 0;
                GameManager.Instance.durationMiniGame = rangeDureeManche[iterator];
                break;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isActive = false;
    }

    public enum RULE
    {
        nbManches,
        dureeManche
    }

}
