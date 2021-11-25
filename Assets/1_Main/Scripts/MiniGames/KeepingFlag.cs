using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepingFlag : MonoBehaviour
{
    private KeepTheFlagManager keepTheFlagScript;

    private Transform flagInitialPos;

    //Security for trigger only one time
    private bool flagIsCaptured = false;

    private void Start()
    {
        keepTheFlagScript = GetComponentInParent<KeepTheFlagManager>();
        flagInitialPos = transform;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !flagIsCaptured)
        {
            flagIsCaptured = true;
            AttachFlagToPlayer(collision.transform);

            switch (collision.GetComponent<PlayerController>().playerID)
            {
                case 0:
                    keepTheFlagScript.FlagCaptured(0);
                    break;
                case 1:
                    keepTheFlagScript.FlagCaptured(1);
                    break;
                case 2:
                    keepTheFlagScript.FlagCaptured(2);
                    break;
                case 3:
                    keepTheFlagScript.FlagCaptured(3);
                    break;
            }
        }
    }

    public void AttachFlagToPlayer(Transform player)
    {
        transform.position = player.position;
        transform.SetParent(player);
    }

    public void ResetFlag()
    {
        Destroy(this.gameObject);
        //transform.position = flagInitialPos.position;
    }
}
