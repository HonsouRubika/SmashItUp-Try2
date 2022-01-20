using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewSkinManager : MonoBehaviour
{
    public float waitTime = 3;
    private float waitTimeActu;

    void Start()
    {
        waitTimeActu = waitTime + Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > waitTimeActu) SceneManager.LoadScene("StartScene"); //return to lobby
    }
}
