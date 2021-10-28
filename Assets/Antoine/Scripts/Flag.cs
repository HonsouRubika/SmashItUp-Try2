using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour
{
    private CaptureTheFlag_Rules captureTheFlagRulesScript;

    //Security for trigger only one time
    private bool flagIsCaptured = false;

    private void Start()
    {
        captureTheFlagRulesScript = GetComponentInParent<CaptureTheFlag_Rules>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !flagIsCaptured)
        {
            flagIsCaptured = true;

            if (collision.GetComponent<PlayerController>().playerID == 0)
            {
                captureTheFlagRulesScript.FlagCaptured(0);
            }
            else if (collision.GetComponent<PlayerController>().playerID == 1)
            {
                captureTheFlagRulesScript.FlagCaptured(1);
            }
            else if (collision.GetComponent<PlayerController>().playerID == 2)
            {
                captureTheFlagRulesScript.FlagCaptured(2);
            }
            else if (collision.GetComponent<PlayerController>().playerID == 3)
            {
                captureTheFlagRulesScript.FlagCaptured(3);
            }
        }
    }
}
