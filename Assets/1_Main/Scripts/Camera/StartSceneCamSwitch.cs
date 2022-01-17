using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSceneCamSwitch : MonoBehaviour
{
    public GameObject MainCamera;
    public GameObject FocusSkins;

    private bool isOut = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && !isOut)
        {
            MainCamera.SetActive(false);
            FocusSkins.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            MainCamera.SetActive(true);
            FocusSkins.SetActive(false);

            isOut = true;
        }
    }
}
