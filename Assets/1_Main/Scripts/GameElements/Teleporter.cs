using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    private TeleporterSound teleportSoundScript;

    public enum direction {Vertical, Horizontal};
    public direction teleporterDirection = direction.Vertical;

    public Vector2 transformAddedOnExit;
    public Transform otherTP;

    private void Start()
    {
        teleportSoundScript = GetComponentInChildren<TeleporterSound>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (otherTP != null)
        {
            if (collision.CompareTag("Player"))
            {
                TeleportPlayer(collision);
                //teleportSoundScript.PlayerTeleported();
            }
        }
    }

    private void TeleportPlayer(Collider2D collision)
    {
        switch (teleporterDirection)
        {
            case direction.Vertical:
                collision.transform.position = new Vector2(collision.transform.position.x, otherTP.position.y + transformAddedOnExit.y);
                break;
            case direction.Horizontal:
                collision.transform.position = new Vector2(otherTP.position.x + transformAddedOnExit.x, collision.transform.position.y);
                break;
        } 
    }
}
