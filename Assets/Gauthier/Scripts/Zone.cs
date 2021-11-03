using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone : MonoBehaviour
{
    public bool player0IsInZone = false;
    public bool player1IsInZone = false;
    public bool player2IsInZone = false;
    public bool player3IsInZone = false;
    public int counterPlayerinZone = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (collision.GetComponent<PlayerController>().playerID == 0)
            {
                player0IsInZone = true;
                counterPlayerinZone++;

            }

            if (collision.GetComponent<PlayerController>().playerID == 1)
            {
                player1IsInZone = true;
                counterPlayerinZone++;
            }

            if (collision.GetComponent<PlayerController>().playerID == 2)
            {
                player2IsInZone = true;
                counterPlayerinZone++;
            }

            if (collision.GetComponent<PlayerController>().playerID == 3)
            {
                player3IsInZone = true;
                counterPlayerinZone++;
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
                counterPlayerinZone--;
            }

            if (collision.GetComponent<PlayerController>().playerID == 1)
            {
                player1IsInZone = false;
                counterPlayerinZone--;
            }

            if (collision.GetComponent<PlayerController>().playerID == 2)
            {
                player2IsInZone = false;
                counterPlayerinZone--;
            }

            if (collision.GetComponent<PlayerController>().playerID == 3)
            {
                player3IsInZone = false;
                counterPlayerinZone--;
            }
        }
    }
}
