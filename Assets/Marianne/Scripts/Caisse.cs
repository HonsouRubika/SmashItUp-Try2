using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Caisse : MonoBehaviour
{ 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Input.GetKey("Attack"))
         {
            MiniJeu_DestructCaisse.MJDC.nbrdecaisse -= 1;
            //GetComponent<PlayerController>().playerID;
        }
    }
}
