using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpening : MonoBehaviour
{
    public GameObject openDoor;
    public GameObject closeDoor;

    public DoorSound DoorSoundScript;

    private void Start()
    {
        openDoor.SetActive(false);
        closeDoor.SetActive(true);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            openDoor.SetActive(true);
            closeDoor.SetActive(false);

            DoorSoundScript.DoorOpening();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            openDoor.SetActive(false);
            closeDoor.SetActive(true);
        }
    }
}