using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewSkinManager : MonoBehaviour
{
    public float waitTime = 3;
    private float waitTimeActu;

    public GameObject unlock;
    public Sprite[] unlockPossibility;

    public GameObject skinDisplay;
    public GameObject[] unlockableSkins;
    public Sprite[] unlockableSkinsSprite;
    private bool isSetup = false;

    void Start()
    {
        waitTimeActu = waitTime + Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isSetup)
        {
            UnlockNewScene();
        }
        if (Time.time > waitTimeActu) SceneManager.LoadScene("StartScene"); //return to lobby
    }

    private void UnlockNewScene()
    {

        for(int i = 0; i< GameManager.Instance.scoreValuesManagerScript.players.Length; i++)
        {
            //TODO : utiliser la liste de nouveau skin à débloquer "unlockableSkins"
            switch (GameManager.Instance.nbGameFinished) {
                case 2:
                    GameManager.Instance.players[i].GetComponent<PlayerSkins>().NewSkinUnlocked(unlockableSkins[0]);
                    unlock.GetComponent<SpriteRenderer>().sprite = unlockPossibility[0];
                    skinDisplay.GetComponent<SpriteRenderer>().sprite = unlockableSkinsSprite[0];
                    break;
                case 4:
                    GameManager.Instance.players[i].GetComponent<PlayerSkins>().NewSkinUnlocked(unlockableSkins[1]);
                    unlock.GetComponent<SpriteRenderer>().sprite = unlockPossibility[1];
                    skinDisplay.GetComponent<SpriteRenderer>().sprite = unlockableSkinsSprite[0];
                    break;
            }
            GameManager.Instance.scoreValuesManagerScript.players[i].GetComponent<PlayerSkins>().NewSkinUnlocked(new GameObject());
        }
    }
}
