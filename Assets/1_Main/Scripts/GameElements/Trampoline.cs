using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampoline : MonoBehaviour
{

    private bool isEffective = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Check if Player is jumping on the button
        if (collision.CompareTag("Player") && collision.GetComponent<PlayerController>().jumpState == JumpState.Falling && isEffective)
        {
            isEffective = false;
            collision.GetComponent<PlayerController>().computeTrampolineJump();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isEffective = true;
    }

}
