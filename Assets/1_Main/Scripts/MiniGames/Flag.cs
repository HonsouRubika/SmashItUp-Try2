using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour
{
    private FlagSound flagSoundScript;
    private CaptureTheFlag_Rules captureTheFlagRulesScript;

    //Security for trigger only one time
    private bool flagIsCaptured = false;

    private void Start()
    {
        captureTheFlagRulesScript = GetComponentInParent<CaptureTheFlag_Rules>();

       
            flagSoundScript = GetComponentInChildren<FlagSound>();
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !flagIsCaptured)
        {
            flagIsCaptured = true;
            flagSoundScript.PlayerTakeFlag();

            switch (collision.GetComponent<PlayerController>().playerID)
            {
                case 0:
                    captureTheFlagRulesScript.FlagCaptured(0);
                    break;
                case 1:
                    captureTheFlagRulesScript.FlagCaptured(1);
                    break;
                case 2:
                    captureTheFlagRulesScript.FlagCaptured(2);
                    break;
                case 3:
                    captureTheFlagRulesScript.FlagCaptured(3);
                    break;
            }

            Destroy(gameObject);
        }
    }
}
