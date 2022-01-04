using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerSkins : MonoBehaviour
{
    //ChangePlayerArea
    private bool isInArea = false;

    public GameObject baseSkin;
    public Transform skinPosition; 

    [Header("Current Skin")]
    public GameObject currentSkin;

    [Header("All skins")]
    public List<GameObject> skins;

    [Header("Color players")]
    public Color colorP1;
    public Color colorP2;
    public Color colorP3;
    public Color colorP4;

    [Header("Players Cursor")]
    public Sprite P1;
    public Sprite P2;
    public Sprite P3;
    public Sprite P4;

    private int skinNumber;

    private PlayerAnim playerAnimScript;
    private Transform parent;
    private PlayerController playerControllerScript;

    //timer pour debug
    private float changerSkinTimer = 0.5f;
    private float changerSkinTimerActu;

    private void Start()
    {
        changerSkinTimerActu = Time.time;

        parent = GetComponentInParent<PlayerController>().transform;
        playerAnimScript = GetComponent<PlayerAnim>();
        playerControllerScript = parent.GetComponent<PlayerController>();

        Transform[] children = gameObject.GetComponentsInChildren<Transform>(true);
        foreach (Transform item in children)
        {
            if (item.tag == "skin")
            {
                currentSkin = item.gameObject;
            }
        }

        playerAnimScript.playerAnimator = currentSkin.GetComponent<Animator>();
        playerControllerScript.playerAnimator = currentSkin.GetComponent<Animator>().transform;

        SetColorToPlayer(currentSkin);
    }

    public void ChangeSkin(InputAction.CallbackContext context)
    {
        // change skin only in hub
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Test"))
        {
            //https://issuetracker.unity3d.com/issues/input-system-unity-events-called-twice-when-using-player-input-manager-and-player-input
            //je dois utiliser un timer parce que le new input system est buggé => la fonction est call deux fois

            if (context.started && Time.time >= changerSkinTimerActu)
            {
                changerSkinTimerActu = Time.time + changerSkinTimer;

                skinNumber++;

                if (skinNumber >= skins.Count)
                {
                    skinNumber = 0;
                }

                //on supprime le skin actuel
                Destroy(currentSkin);

                //on ajoute le nouveau skin
                currentSkin = Instantiate(skins[skinNumber], skinPosition.position, Quaternion.identity, parent);
                if (skinNumber == 0)
                {
                    SetColorToPlayer(currentSkin);
                }
                
                if (currentSkin.GetComponent<Animator>() == null) Debug.Log("error");
                playerAnimScript.playerAnimator = currentSkin.GetComponent<Animator>();
                playerControllerScript.playerAnimator = currentSkin.GetComponent<Animator>().transform;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "ChangeSkinArea")
        {
            isInArea = true;
        }
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name == "ChangeSkinArea")
        {
            isInArea = false;
        }
    }

    public void ChangeSkinPlus(InputAction.CallbackContext context)
    {
        if (context.started && isInArea && (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Test") || SceneManager.GetActiveScene() == SceneManager.GetSceneByName("SceneTestPlateforme")))
        {
            skinNumber++;
            if (skinNumber >= skins.Count)
            {
                skinNumber = 0;
            }

            //on supprime le skin actuel
            Destroy(currentSkin);

            //on ajoute le nouveau skin
            currentSkin = Instantiate(skins[skinNumber], skinPosition.position, Quaternion.identity, parent);
            if (skinNumber == 0)
            {
                SetColorToPlayer(currentSkin);
            }

            if (currentSkin.GetComponent<Animator>() == null) Debug.Log("error");
            playerAnimScript.playerAnimator = currentSkin.GetComponent<Animator>();
            playerControllerScript.playerAnimator = currentSkin.GetComponent<Animator>().transform;
        }
    }
    
    public void ChangeSkinMinus(InputAction.CallbackContext context)
    {
        if (context.started && isInArea && (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Test") || SceneManager.GetActiveScene() == SceneManager.GetSceneByName("SceneTestPlateforme")))
        {
            skinNumber--;
            if (skinNumber < 0)
            {
                skinNumber = skins.Count - 1;
            }

            //on supprime le skin actuel
            Destroy(currentSkin);

            //on ajoute le nouveau skin
            currentSkin = Instantiate(skins[skinNumber], skinPosition.position, Quaternion.identity, parent);
            if (skinNumber == 0)
            {
                SetColorToPlayer(currentSkin);
            }

            if (currentSkin.GetComponent<Animator>() == null) Debug.Log("error");
            playerAnimScript.playerAnimator = currentSkin.GetComponent<Animator>();
            playerControllerScript.playerAnimator = currentSkin.GetComponent<Animator>().transform;
        }
    }

    private void SetColorToPlayer(GameObject skin)
    {
        switch (playerControllerScript.playerID)
        {
            case 0:
                transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = P1;

                SpriteRenderer[] spritesP1 =  skin.GetComponentsInChildren<SpriteRenderer>();
                for (int i = 0; i < spritesP1.Length; i++)
                {
                    spritesP1[i].material.SetColor("_Color", colorP1);
                }
                break;
            case 1:
                transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = P2;

                SpriteRenderer[] spritesP2 = skin.GetComponentsInChildren<SpriteRenderer>();
                for (int i = 0; i < spritesP2.Length; i++)
                {
                    spritesP2[i].material.SetColor("_Color", colorP2);
                }
                break;
            case 2:
                transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = P3;

                SpriteRenderer[] spritesP3 = skin.GetComponentsInChildren<SpriteRenderer>();
                for (int i = 0; i < spritesP3.Length; i++)
                {
                    spritesP3[i].material.SetColor("_Color", colorP3);
                }
                break;
            case 3:
                transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = P4;

                SpriteRenderer[] spritesP4 = skin.GetComponentsInChildren<SpriteRenderer>();
                for (int i = 0; i < spritesP4.Length; i++)
                {
                    spritesP4[i].material.SetColor("_Color", colorP4);
                }
                break;
        }
    }
}
