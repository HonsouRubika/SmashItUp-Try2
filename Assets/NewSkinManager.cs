using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewSkinManager : MonoBehaviour
{
    public float waitTime = 3;
    private float waitTimeActu;

    public GameObject[] unlockableSkins;

    void Start()
    {
        waitTimeActu = waitTime + Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > waitTimeActu) SceneManager.LoadScene("StartScene"); //return to lobby
    }

    private void UnlockNewScene()
    {
        for(int i = 0; i< GameManager.Instance.scoreValuesManagerScript.players.Length; i++)
        {
            //TODO : utiliser la liste de nouveau skin à débloquer "unlockableSkins"
            GameManager.Instance.scoreValuesManagerScript.players[i].GetComponent<PlayerSkins>().NewSkinUnlocked(new GameObject());
        }
    }
}
