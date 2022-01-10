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
    public Color blue;
    public Color red;
    public Color green;
    public Color yellow;

    [Header("Players Cursor")]
    public Sprite P1;
    public Sprite P2;
    public Sprite P3;
    public Sprite P4;

    [Header("Players hammer")]
    public Sprite hammerP1;
    public Sprite hammerP2;
    public Sprite hammerP3;
    public Sprite hammerP4;

    public SpriteRenderer[] skinSprites;

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
                //on link les points du hammer
                GameObject hammer = GetChildWithName(currentSkin, "Hammer");
                playerControllerScript.attackPointL = GetChildWithName(hammer, "AttackPointL").transform;
                playerControllerScript.attackPointR = GetChildWithName(hammer, "AttackPointR").transform;
                playerControllerScript.hammerPointL = GetChildWithName(hammer, "hammerPointL");
                playerControllerScript.hammerPointR = GetChildWithName(hammer, "hammerPointR");
                //playerControllerScript.attackPointL = currentSkin.GetComponentInChildren<GameObject>("").transform;

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
        if (context.started && isInArea)
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
            //on link les points du hammer
            /*foreach (var point in currentSkin.GetComponentsInChildren<GameObject>())
            {
                if (point.name == "AttackPointL")
                {
                    Debug.Log("point found");
                    playerControllerScript.attackPointL = point.transform;
                }
                if (point.name == "AttackPointR")
                {
                    playerControllerScript.attackPointR = point.transform;
                }
                if (point.name == "hammerPointL")
                {
                    playerControllerScript.hammerPointL = point;
                }
                if (point.name == "hammerPointR")
                {
                    playerControllerScript.hammerPointR = point;
                }
            }*/
            //if (GetChildWithName(currentSkin, "Hammer") != null) Debug.Log("yesss");
            GameObject hammer = GetChildWithName(currentSkin, "Hammer");
            playerControllerScript.attackPointL = GetChildWithName(hammer, "AttackPointL").transform;
            playerControllerScript.attackPointR = GetChildWithName(hammer, "AttackPointR").transform;
            playerControllerScript.hammerPointL = GetChildWithName(hammer, "hammerPointL");
            playerControllerScript.hammerPointR = GetChildWithName(hammer, "hammerPointR");

            playerControllerScript.hammerPointL.SetActive(false);
            playerControllerScript.hammerPointR.SetActive(false);


            playerAnimScript.playerAnimator = currentSkin.GetComponent<Animator>();
            playerControllerScript.playerAnimator = currentSkin.GetComponent<Animator>().transform;
        }
    }

    public void ChangeSkinMinus(InputAction.CallbackContext context)
    {
            if (context.started && isInArea)
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
            //on link les points du hammer
            /*foreach (var point in currentSkin.GetComponentsInChildren<GameObject>())
            {
                if (point.name == "AttackPointL")
                {
                    Debug.Log("point found");
                    playerControllerScript.attackPointL = point.transform;
                }
                if (point.name == "AttackPointR")
                {
                    playerControllerScript.attackPointR = point.transform;
                }
                if (point.name == "hammerPointL")
                {
                    playerControllerScript.hammerPointL = point;
                }
                if (point.name == "hammerPointR")
                {
                    playerControllerScript.hammerPointR = point;
                }
            }*/
            //if (GetChildWithName(currentSkin, "Hammer") != null) Debug.Log("yesss");
            GameObject hammer = GetChildWithName(currentSkin, "Hammer");
            playerControllerScript.attackPointL = GetChildWithName(hammer, "AttackPointL").transform;
            playerControllerScript.attackPointR = GetChildWithName(hammer, "AttackPointR").transform;
            playerControllerScript.hammerPointL = GetChildWithName(hammer, "hammerPointL");
            playerControllerScript.hammerPointR = GetChildWithName(hammer, "hammerPointR");

            if (currentSkin.GetComponent<Animator>() == null) Debug.Log("error");
            playerAnimScript.playerAnimator = currentSkin.GetComponent<Animator>();
            playerControllerScript.playerAnimator = currentSkin.GetComponent<Animator>().transform;
        }
    }
    GameObject GetChildWithName(GameObject obj, string name)
    {
        Transform trans = obj.transform;
        Transform childTrans = trans.Find(name);
        if (childTrans != null)
        {
            return childTrans.gameObject;
        }
        else
        {
            return null;
        }
    }

    private void SetColorToPlayer(GameObject skin)
    {
        switch (playerControllerScript.playerID)
        {
            case 0:
                transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = P1;
                currentSkin.transform.Find("Hammer").GetComponent<SpriteRenderer>().sprite = hammerP1;

                /*SpriteRenderer[] spritesP1 =  skin.GetComponentsInChildren<SpriteRenderer>();
                for (int i = 0; i < spritesP1.Length; i++)
                {
                    spritesP1[i].material.SetColor("_Color", blue);
                }*/
                break;
            case 1:
                transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = P2;
                currentSkin.transform.Find("Hammer").GetComponent<SpriteRenderer>().sprite = hammerP2;

                SpriteRenderer[] spritesP2 = skin.GetComponentsInChildren<SpriteRenderer>();
                /*for (int i = 0; i < spritesP2.Length; i++)
                {
                    spritesP2[i].material.SetColor("_Color", red);
                }*/
                break;
            case 2:
                transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = P3;
                currentSkin.transform.Find("Hammer").GetComponent<SpriteRenderer>().sprite = hammerP3;

                SpriteRenderer[] spritesP3 = skin.GetComponentsInChildren<SpriteRenderer>();
                /*for (int i = 0; i < spritesP3.Length; i++)
                {
                    spritesP3[i].material.SetColor("_Color", green);
                }*/
                break;
            case 3:
                transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = P4;
                currentSkin.transform.Find("Hammer").GetComponent<SpriteRenderer>().sprite = hammerP4;

                SpriteRenderer[] spritesP4 = skin.GetComponentsInChildren<SpriteRenderer>();
                /*for (int i = 0; i < spritesP4.Length; i++)
                {
                    spritesP4[i].material.SetColor("_Color", yellow);
                }*/
                break;
        }
    }

    public void SetColorByTeam(string color)
    {
        skinSprites = currentSkin.GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < skinSprites.Length; i++)
        {
            skinSprites[i].material.SetFloat("_Intensity", 0.75f);
            switch (color)
            {
                case "blue":
                    skinSprites[i].material.SetColor("_Color", blue);
                    break;
                case "red":
                    skinSprites[i].material.SetColor("_Color", red);
                    break;
                case "green":
                    skinSprites[i].material.SetColor("_Color", green);
                    break;
                case "yellow":
                    skinSprites[i].material.SetColor("_Color", yellow);
                    break;
            } 
        }
    }
}
