using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone : MonoBehaviour
{
    public bool player0IsInZone = false;
    public bool player1IsInZone = false;
    //public bool player2IsInZone = false;
    //public bool player3IsInZone = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (collision.GetComponent<PlayerController>().playerID == 0)
            {
                player0IsInZone = true;
            }

            if (collision.GetComponent<PlayerController>().playerID == 1)
            {
                player1IsInZone = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (collision.GetComponent<PlayerController>().playerID == 0)
            {
                player0IsInZone = false;
            }

            if (collision.GetComponent<PlayerController>().playerID == 1)
            {
                player1IsInZone = false;
            }
        }
    }
}
