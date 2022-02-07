using System.Collections;
using System.Collections.Generic;
using System;
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
    public bool[] isSkinUnlocked;

    [Header("Color Team")]
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
    public Sprite goldenHammerP1;
    public Sprite goldenHammerP2;
    public Sprite goldenHammerP3;
    public Sprite goldenHammerP4;

    [Header("Team hammer")]
    public Sprite purpleHammer;
    public Sprite orangeHammer;
    public Sprite goldenPurpleHammer;
    public Sprite goldenOrangeHammer;

    [Header("Team Cursor")]
    public Sprite purpleCursor;
    public Sprite orangeCursor;
    private GameObject cursorTeam;

    [Header("Team Halo")]
    public Sprite purpleHalo;
    public Sprite orangeHalo;
    private GameObject haloTeam;

    [Header("Skin Display StartScene")]
    public Sprite[] P1Skins;
    public Sprite[] P2Skins;
    public Sprite[] P3Skins;
    public Sprite[] P4Skins;
    private GameObject[] skinDisplay;
    private IEnumerable<GameObject> orderedSkinDisplay;
    private bool isInStartScene = false;
    private bool isDisplaySetup = false;

    [HideInInspector] public GameObject currentHammer;
    private GameObject previousHammer;
    private Color hammerColor;

    public SpriteRenderer[] skinSprites;

    public int skinNumber;

    private PlayerAnim playerAnimScript;
    private Transform parent;
    private PlayerController playerControllerScript;

    //timer pour debug
    private float changerSkinTimer = 0.5f;
    private float changerSkinTimerActu;

    //playtest
    private uint overlayPreset = 0;

    //FX
    private PlayerFX playerFXScript;

    private void Start()
    {
        cursorTeam = transform.GetChild(6).gameObject;
        haloTeam = transform.GetChild(7).gameObject;
        SetCursorTeam("default");
        SetHaloTeam("default");

        skinDisplay = new GameObject[4];

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

        //not visible in start scene from the start
        if(SceneManager.GetActiveScene().name == "NewStartScene")
        {
            currentSkin.SetActive(false);
        }
        

        isSkinUnlocked = new bool[skins.Count];
        //starter skins
        for (int i = 0; i< 4; i++)
        {
            isSkinUnlocked[i] = true;
        }


        /*if (GameManager.Instance.nbGameFinished >= 1)
        {
            isSkinUnlocked[4] = true;
            //Debug.Log("skin 5 unlocked");
        } else
        {
            //Debug.Log("skin 5 locked");
        }
        if (GameManager.Instance.nbGameFinished >= 3)
        {
            isSkinUnlocked[5] = true;
        }*/

        if (SceneManager.GetActiveScene().name == "NewStartScene")
        {
            isInArea = true;
            isInStartScene = true;
            skinDisplay = GameObject.FindGameObjectsWithTag("SkinDisplay");
            //orderedSkinDisplay = skinDisplay.OrderBy(go => go.name);
            Array.Sort(skinDisplay, (a, b) => a.name.CompareTo(b.name));

            //Start scene skin desplay
            switch (playerControllerScript.playerID)
            {
                case 0:
                    skinDisplay[0].GetComponent<SpriteRenderer>().sprite = P1Skins[skinNumber];
                    break;
                case 1:
                    skinDisplay[1].GetComponent<SpriteRenderer>().sprite = P2Skins[skinNumber];
                    break;
                case 2:
                    skinDisplay[2].GetComponent<SpriteRenderer>().sprite = P3Skins[skinNumber];
                    break;
                case 3:
                    skinDisplay[3].GetComponent<SpriteRenderer>().sprite = P4Skins[skinNumber];
                    break;
            }
        }


        playerAnimScript.playerAnimator = currentSkin.GetComponent<Animator>();
        playerControllerScript.playerAnimator = currentSkin.GetComponent<Animator>().transform;
        currentHammer = currentSkin.transform.Find("Hammer").gameObject;
        hammerColor = currentHammer.GetComponent<SpriteRenderer>().color;

        SetColorToPlayer(currentSkin);
        setSkinColorOnSwitchingSkin();

        playerFXScript = GetComponent<PlayerFX>();
        playerFXScript.FindParticlesSystems(currentSkin, currentHammer);
    }

    //FOR PLAYTEST PURPOSES
    private void Update()
    {

        if (Keyboard.current.oKey.wasPressedThisFrame && GameManager.Instance.playersTeam.Length > 0)
        {
            overlayPreset++;
            if (overlayPreset == 3) overlayPreset = 0;

            //change preset
            Debug.Log("change overlay preset");

            switch (overlayPreset)
            {
                case 0: //halo seul
                    Debug.Log("0");
                    cursorTeam.SetActive(false);
                    haloTeam.SetActive(true);
                    SetHammerColorByTeam("default");
                    break;
                case 1: //halo et fleche
                    Debug.Log("1");
                    cursorTeam.SetActive(true);
                    SetHammerColorByTeam("default");
                    switch (GameManager.Instance.playersTeam[playerControllerScript.playerID])
                    {
                        case 0:
                            haloTeam.SetActive(true);
                            break;
                        case 1:
                            haloTeam.SetActive(true);
                            break;
                        default:
                            haloTeam.SetActive(false);
                            break;
                    }
                    break;
                case 2: //halo et marteau
                    Debug.Log("2");
                    cursorTeam.SetActive(false);
                    //hammer true on change
                    switch (GameManager.Instance.playersTeam[playerControllerScript.playerID])
                    {
                        case 0:
                            haloTeam.SetActive(true);
                            SetHammerColorByTeam("purple");
                            break;
                        case 1:
                            haloTeam.SetActive(true);
                            SetHammerColorByTeam("orange");
                            break;
                        default:
                            haloTeam.SetActive(false);
                            SetHammerColorByTeam("default");
                            break;
                    }
                    break;
            }
        }

        if (isInStartScene && SceneManager.GetActiveScene().name != "NewStartScene")
        {
            isInStartScene = false;
            isInArea = false;
        }
        else if (SceneManager.GetActiveScene().name == "NewStartScene")
        {
            isInStartScene = true;
            isInArea = true;
        }

    }

    /*
    //OLD FONCTION
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
                playerAnimScript.SetAnimCooldownAttack();
                playerControllerScript.playerAnimator = currentSkin.GetComponent<Animator>().transform;
                currentHammer = currentSkin.transform.Find("Hammer").gameObject;
                hammerColor = currentHammer.GetComponent<SpriteRenderer>().color;
            }
        }
    }
    */

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

    public void NewSkinUnlocked(GameObject skin)
    {
        switch (GameManager.Instance.nbGameFinished)
        {
            case 1:
                isSkinUnlocked[4] = true;
                break;
            case 3:
                isSkinUnlocked[5] = true;
                break;
        }
    }

    public void ChangeSkinPlus(InputAction.CallbackContext context)
    {
        if (context.started && isInArea && !GameManager.Instance.isPaused)
        {
            skinNumber++;
            if (skinNumber >= skins.Count)
            {
                skinNumber = 0;
            }
            if (!isSkinUnlocked[skinNumber])
            {
                while (!isSkinUnlocked[skinNumber])
                {
                    skinNumber++;
                    if (skinNumber >= skins.Count)
                    {
                        skinNumber = 0;
                    }
                }
            }

            //Start scene skin desplay
            if(isInStartScene)
            {
                //depop player
                playerControllerScript.PlayerDepop();

                switch (playerControllerScript.playerID)
                {
                    case 0:
                        skinDisplay[0].GetComponent<SpriteRenderer>().sprite = P1Skins[skinNumber];
                        break;
                    case 1:
                        skinDisplay[1].GetComponent<SpriteRenderer>().sprite = P2Skins[skinNumber];
                        break;
                    case 2:
                        skinDisplay[2].GetComponent<SpriteRenderer>().sprite = P3Skins[skinNumber];
                        break;
                    case 3:
                        skinDisplay[3].GetComponent<SpriteRenderer>().sprite = P4Skins[skinNumber];
                        break;
                }
            }

            //Save previous hammer
            previousHammer = currentHammer;

            //on supprime le skin actuel
            Destroy(currentSkin);

            //on ajoute le nouveau skin
            currentSkin = Instantiate(skins[skinNumber], skinPosition.position, Quaternion.identity, parent);
            if (skinNumber == 0)
            {
                SetColorToPlayer(currentSkin);
            }

            setHammerColorOnSwitchingSkin();
            setSkinColorOnSwitchingSkin();

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
            playerControllerScript.hammerFXSpawn = GetChildWithName(hammer, "hammerFXSpawn").transform;
            playerControllerScript.hammerFXSpawnEmpty = GetChildWithName(hammer, "hammerFXSpawnEmpty").transform;
            playerControllerScript.attackPointL = GetChildWithName(hammer, "AttackPointL").transform;
            playerControllerScript.attackPointR = GetChildWithName(hammer, "AttackPointR").transform;
            playerControllerScript.hammerPointL = GetChildWithName(hammer, "hammerPointL");
            playerControllerScript.hammerPointR = GetChildWithName(hammer, "hammerPointR");

            playerControllerScript.hammerPointL.SetActive(false);
            playerControllerScript.hammerPointR.SetActive(false);

            playerAnimScript.playerAnimator = currentSkin.GetComponent<Animator>();
            playerAnimScript.SetAnimCooldownAttack();
            playerControllerScript.playerAnimator = currentSkin.GetComponent<Animator>().transform;
            currentHammer = currentSkin.transform.Find("Hammer").gameObject;
            hammerColor = currentHammer.GetComponent<SpriteRenderer>().color;

            playerFXScript.FindParticlesSystems(currentSkin, currentHammer);
        }
    }

    public void ChangeSkinMinus(InputAction.CallbackContext context)
    {
        if (context.started && isInArea && !GameManager.Instance.isPaused)
            {
            skinNumber--;
            if (skinNumber < 0)
            {
                skinNumber = skins.Count - 1;
            }
            if (!isSkinUnlocked[skinNumber])
            {
                while (!isSkinUnlocked[skinNumber])
                {
                    skinNumber--;
                    if (skinNumber < 0)
                    {
                        skinNumber = skins.Count - 1;
                    }
                }
            }

            //Start scene skin desplay
            if (isInStartScene)
            {
                //depop player
                playerControllerScript.PlayerDepop();

                switch (playerControllerScript.playerID)
                {
                    case 0:
                        skinDisplay[0].GetComponent<SpriteRenderer>().sprite = P1Skins[skinNumber];
                        break;
                    case 1:
                        skinDisplay[1].GetComponent<SpriteRenderer>().sprite = P2Skins[skinNumber];
                        break;
                    case 2:
                        skinDisplay[2].GetComponent<SpriteRenderer>().sprite = P3Skins[skinNumber];
                        break;
                    case 3:
                        skinDisplay[3].GetComponent<SpriteRenderer>().sprite = P4Skins[skinNumber];
                        break;
                }
            }

            /*
            skinNumber++;
            if (skinNumber >= skins.Count && isSkinUnlocked[skinNumber])
            {
                skinNumber = 0;
            }
            else if (!isSkinUnlocked[skinNumber])
            {
                while (!isSkinUnlocked[skinNumber])
                {
                    skinNumber++;
                    if (skinNumber >= skins.Count)
                    {
                        skinNumber = 0;
                    }
                }
            }
            */

            //Save previous hammer
            previousHammer = currentHammer;

            //on supprime le skin actuel
            Destroy(currentSkin);

            //on ajoute le nouveau skin
            currentSkin = Instantiate(skins[skinNumber], skinPosition.position, Quaternion.identity, parent);
            if (skinNumber == 0)
            {             
                SetColorToPlayer(currentSkin);
            }

            setHammerColorOnSwitchingSkin();
            setSkinColorOnSwitchingSkin();

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
            playerControllerScript.hammerFXSpawn = GetChildWithName(hammer, "hammerFXSpawn").transform;
            playerControllerScript.hammerFXSpawnEmpty = GetChildWithName(hammer, "hammerFXSpawnEmpty").transform;
            playerControllerScript.attackPointL = GetChildWithName(hammer, "AttackPointL").transform;
            playerControllerScript.attackPointR = GetChildWithName(hammer, "AttackPointR").transform;
            playerControllerScript.hammerPointL = GetChildWithName(hammer, "hammerPointL");
            playerControllerScript.hammerPointR = GetChildWithName(hammer, "hammerPointR");

            playerControllerScript.hammerPointL.SetActive(false);
            playerControllerScript.hammerPointR.SetActive(false);

            if (currentSkin.GetComponent<Animator>() == null) Debug.Log("error");
            playerAnimScript.playerAnimator = currentSkin.GetComponent<Animator>();
            playerAnimScript.SetAnimCooldownAttack();
            playerControllerScript.playerAnimator = currentSkin.GetComponent<Animator>().transform;
            currentHammer = currentSkin.transform.Find("Hammer").gameObject;
            hammerColor = currentHammer.GetComponent<SpriteRenderer>().color;

            playerFXScript.FindParticlesSystems(currentSkin, currentHammer);
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

    private void SetHammerColor()
    {
        switch (playerControllerScript.playerID)
        {
            case 0:
                currentHammer.GetComponent<SpriteRenderer>().sprite = hammerP1;
                break;
            case 1:
                currentHammer.GetComponent<SpriteRenderer>().sprite = hammerP2;
                break;
            case 2:
                currentHammer.GetComponent<SpriteRenderer>().sprite = hammerP3;
                break;
            case 3:
                currentHammer.GetComponent<SpriteRenderer>().sprite = hammerP4;
                break;
        }
    }

    private void SetColorToPlayer(GameObject skin)
    {
        switch (playerControllerScript.playerID)
        {
            case 0:
                transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = P1;
                transform.GetChild(5).GetComponent<SpriteRenderer>().color = blue;
                currentSkin.transform.Find("Hammer").GetComponent<SpriteRenderer>().sprite = hammerP1;

                /*SpriteRenderer[] spritesP1 =  skin.GetComponentsInChildren<SpriteRenderer>();
                for (int i = 0; i < spritesP1.Length; i++)
                {
                    spritesP1[i].material.SetColor("_Color", blue);
                }*/
                break;
            case 1:
                transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = P2;
                transform.GetChild(5).GetComponent<SpriteRenderer>().color = red;
                currentSkin.transform.Find("Hammer").GetComponent<SpriteRenderer>().sprite = hammerP2;

                /*SpriteRenderer[] spritesP2 = skin.GetComponentsInChildren<SpriteRenderer>();
                for (int i = 0; i < spritesP2.Length; i++)
                {
                    spritesP2[i].material.SetColor("_Color", red);
                }*/
                break;
            case 2:
                transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = P3;
                transform.GetChild(5).GetComponent<SpriteRenderer>().color = green;
                currentSkin.transform.Find("Hammer").GetComponent<SpriteRenderer>().sprite = hammerP3;

                /*SpriteRenderer[] spritesP3 = skin.GetComponentsInChildren<SpriteRenderer>();
                for (int i = 0; i < spritesP3.Length; i++)
                {
                    spritesP3[i].material.SetColor("_Color", green);
                }*/
                break;
            case 3:
                transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = P4;
                transform.GetChild(5).GetComponent<SpriteRenderer>().color = yellow;
                currentSkin.transform.Find("Hammer").GetComponent<SpriteRenderer>().sprite = hammerP4;

                /*SpriteRenderer[] spritesP4 = skin.GetComponentsInChildren<SpriteRenderer>();
                /for (int i = 0; i < spritesP4.Length; i++)
                {
                    spritesP4[i].material.SetColor("_Color", yellow);
                }*/
                break;
        }
    }

    public void SetSpriteColorByTeam(string color)
    {
        skinSprites = currentSkin.GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < skinSprites.Length; i++)
        {
            switch (color)
            {
                case "blue":
                    skinSprites[i].material.SetColor("_Color", blue);
                    skinSprites[i].material.SetFloat("_Intensity", 0.75f);
                    break;
                case "red":
                    skinSprites[i].material.SetColor("_Color", red);
                    skinSprites[i].material.SetFloat("_Intensity", 0.75f);
                    break;
                case "green":
                    skinSprites[i].material.SetColor("_Color", green);
                    skinSprites[i].material.SetFloat("_Intensity", 0.75f);
                    break;
                case "yellow":
                    skinSprites[i].material.SetColor("_Color", yellow);
                    skinSprites[i].material.SetFloat("_Intensity", 0.75f);
                    break;
                case "default":
                    skinSprites[i].material.SetColor("_Color", Color.white);
                    skinSprites[i].material.SetFloat("_Intensity", 0f);
                    break;
            } 
        }
    }

    public string GetHammerColor()
    {
        if (currentHammer.GetComponent<SpriteRenderer>().sprite == goldenHammerP1
            || currentHammer.GetComponent<SpriteRenderer>().sprite == goldenHammerP2
            || currentHammer.GetComponent<SpriteRenderer>().sprite == goldenHammerP3
            || currentHammer.GetComponent<SpriteRenderer>().sprite == goldenHammerP4)
        {
            return "golden";
        }
        else if (currentHammer.GetComponent<SpriteRenderer>().sprite == hammerP1
            || currentHammer.GetComponent<SpriteRenderer>().sprite == hammerP2
            || currentHammer.GetComponent<SpriteRenderer>().sprite == hammerP3
            || currentHammer.GetComponent<SpriteRenderer>().sprite == hammerP4)
        {
            return "default";
        }
        else if (currentHammer.GetComponent<SpriteRenderer>().sprite == purpleHammer
            || currentHammer.GetComponent<SpriteRenderer>().sprite == goldenPurpleHammer)
        {
            return "purple";
        }
        else if (currentHammer.GetComponent<SpriteRenderer>().sprite == orangeHammer
            || currentHammer.GetComponent<SpriteRenderer>().sprite == goldenOrangeHammer)
        {
            return "orange";
        }
        else
        {
            return "null";
        }
    }

    public void SetHammerColorByTeam(string color)
    {
        switch (color)
        {
            case "purple":
                if (currentSkin.transform.Find("Hammer").GetComponent<SpriteRenderer>().sprite != goldenHammerP1
                    && currentSkin.transform.Find("Hammer").GetComponent<SpriteRenderer>().sprite != goldenHammerP2
                    && currentSkin.transform.Find("Hammer").GetComponent<SpriteRenderer>().sprite != goldenHammerP3
                    && currentSkin.transform.Find("Hammer").GetComponent<SpriteRenderer>().sprite != goldenHammerP4)
                {
                    currentSkin.transform.Find("Hammer").GetComponent<SpriteRenderer>().sprite = purpleHammer;
                }
                else
                {
                    currentSkin.transform.Find("Hammer").GetComponent<SpriteRenderer>().sprite = goldenPurpleHammer;
                }
                break;
            case "orange":
                if (currentSkin.transform.Find("Hammer").GetComponent<SpriteRenderer>().sprite != goldenHammerP1
                    && currentSkin.transform.Find("Hammer").GetComponent<SpriteRenderer>().sprite != goldenHammerP2
                    && currentSkin.transform.Find("Hammer").GetComponent<SpriteRenderer>().sprite != goldenHammerP3
                    && currentSkin.transform.Find("Hammer").GetComponent<SpriteRenderer>().sprite != goldenHammerP4)
                {
                    currentSkin.transform.Find("Hammer").GetComponent<SpriteRenderer>().sprite = orangeHammer;
                }
                else
                {
                    currentSkin.transform.Find("Hammer").GetComponent<SpriteRenderer>().sprite = goldenOrangeHammer;
                }
                break;
            case "golden":
                switch (playerControllerScript.playerID)
                {
                    case 0:
                        currentSkin.transform.Find("Hammer").GetComponent<SpriteRenderer>().sprite = goldenHammerP1;
                        break;
                    case 1:
                        currentSkin.transform.Find("Hammer").GetComponent<SpriteRenderer>().sprite = goldenHammerP2;
                        break;
                    case 2:
                        currentSkin.transform.Find("Hammer").GetComponent<SpriteRenderer>().sprite = goldenHammerP3;
                        break;
                    case 3:
                        currentSkin.transform.Find("Hammer").GetComponent<SpriteRenderer>().sprite = goldenHammerP4;
                        break;
                }
                break;
            case "default":
                if (currentSkin.transform.Find("Hammer").GetComponent<SpriteRenderer>().sprite != goldenHammerP1
                    && currentSkin.transform.Find("Hammer").GetComponent<SpriteRenderer>().sprite != goldenHammerP2
                    && currentSkin.transform.Find("Hammer").GetComponent<SpriteRenderer>().sprite != goldenHammerP3
                    && currentSkin.transform.Find("Hammer").GetComponent<SpriteRenderer>().sprite != goldenHammerP4
                    && currentSkin.transform.Find("Hammer").GetComponent<SpriteRenderer>().sprite != goldenPurpleHammer
                    && currentSkin.transform.Find("Hammer").GetComponent<SpriteRenderer>().sprite != goldenOrangeHammer)
                {
                    switch (playerControllerScript.playerID)
                    {
                        case 0:
                            currentSkin.transform.Find("Hammer").GetComponent<SpriteRenderer>().sprite = hammerP1;
                            break;
                        case 1:
                            currentSkin.transform.Find("Hammer").GetComponent<SpriteRenderer>().sprite = hammerP2;
                            break;
                        case 2:
                            currentSkin.transform.Find("Hammer").GetComponent<SpriteRenderer>().sprite = hammerP3;
                            break;
                        case 3:
                            currentSkin.transform.Find("Hammer").GetComponent<SpriteRenderer>().sprite = hammerP4;
                            break;
                    }
                }
                else if (currentSkin.transform.Find("Hammer").GetComponent<SpriteRenderer>().sprite == goldenPurpleHammer
                    || currentSkin.transform.Find("Hammer").GetComponent<SpriteRenderer>().sprite == goldenOrangeHammer)
                {
                    switch (playerControllerScript.playerID)
                    {
                        case 0:
                            currentSkin.transform.Find("Hammer").GetComponent<SpriteRenderer>().sprite = goldenHammerP1;
                            break;
                        case 1:
                            currentSkin.transform.Find("Hammer").GetComponent<SpriteRenderer>().sprite = goldenHammerP2;
                            break;
                        case 2:
                            currentSkin.transform.Find("Hammer").GetComponent<SpriteRenderer>().sprite = goldenHammerP3;
                            break;
                        case 3:
                            currentSkin.transform.Find("Hammer").GetComponent<SpriteRenderer>().sprite = goldenHammerP4;
                            break;
                    }
                }
                break;
            case "reset":
                switch (playerControllerScript.playerID)
                {
                    case 0:
                        currentSkin.transform.Find("Hammer").GetComponent<SpriteRenderer>().sprite = hammerP1;
                        break;
                    case 1:
                        currentSkin.transform.Find("Hammer").GetComponent<SpriteRenderer>().sprite = hammerP2;
                        break;
                    case 2:
                        currentSkin.transform.Find("Hammer").GetComponent<SpriteRenderer>().sprite = hammerP3;
                        break;
                    case 3:
                        currentSkin.transform.Find("Hammer").GetComponent<SpriteRenderer>().sprite = hammerP4;
                        break;
                }
                break;
        } 
    }

    public void SetCursorTeam(string color)
    {
        switch (color)
        {
            case "purple":
                cursorTeam.SetActive(true);
                cursorTeam.GetComponent<SpriteRenderer>().sprite = purpleCursor;
                break;
            case "orange":
                cursorTeam.SetActive(true);
                cursorTeam.GetComponent<SpriteRenderer>().sprite = orangeCursor;
                break;
            case "default":
                cursorTeam.SetActive(false);
                break;
        }
    }

    public void SetHaloTeam(string color)
    {
        switch (color)
        {
            case "purple":
                haloTeam.SetActive(true);
                haloTeam.GetComponent<SpriteRenderer>().sprite = purpleHalo;
                break;
            case "orange":
                haloTeam.SetActive(true);
                haloTeam.GetComponent<SpriteRenderer>().sprite = orangeHalo;
                break;
            case "default":
                haloTeam.SetActive(false);
                break;
        }
    }

    public void SetHammerOpacity(float alpha)
    {
        hammerColor.a = alpha;
        if(currentHammer != null)
        {
            currentHammer.GetComponent<SpriteRenderer>().color = hammerColor;
        }
    }

    private void setHammerColorOnSwitchingSkin()
    {
        switch (playerControllerScript.playerID)
        {
            case 0:
                if (previousHammer != null && previousHammer.GetComponent<SpriteRenderer>().sprite == goldenHammerP1)
                {
                    currentSkin.transform.Find("Hammer").GetComponent<SpriteRenderer>().sprite = goldenHammerP1;
                }
                else
                {
                    currentSkin.transform.Find("Hammer").GetComponent<SpriteRenderer>().sprite = hammerP1;
                }
                break;
            case 1:
                if (previousHammer != null && previousHammer.GetComponent<SpriteRenderer>().sprite == goldenHammerP2)
                {
                    currentSkin.transform.Find("Hammer").GetComponent<SpriteRenderer>().sprite = goldenHammerP2;
                }
                else
                {
                    currentSkin.transform.Find("Hammer").GetComponent<SpriteRenderer>().sprite = hammerP2;
                }
                break;
            case 2:
                if (previousHammer != null && previousHammer.GetComponent<SpriteRenderer>().sprite == goldenHammerP3)
                {
                    currentSkin.transform.Find("Hammer").GetComponent<SpriteRenderer>().sprite = goldenHammerP3;
                }
                else
                {
                    currentSkin.transform.Find("Hammer").GetComponent<SpriteRenderer>().sprite = hammerP3;
                }
                break;
            case 3:
                if (previousHammer != null && previousHammer.GetComponent<SpriteRenderer>().sprite == goldenHammerP4)
                {
                    currentSkin.transform.Find("Hammer").GetComponent<SpriteRenderer>().sprite = goldenHammerP4;
                }
                else
                {
                    currentSkin.transform.Find("Hammer").GetComponent<SpriteRenderer>().sprite = hammerP4;
                }
                break;
        }
    }

    private void setSkinColorOnSwitchingSkin()
    {
        switch (playerControllerScript.playerID)
        {
            case 0:
                currentSkin.transform.Find("P1").gameObject.SetActive(true);
                currentSkin.transform.Find("P2").gameObject.SetActive(false);
                currentSkin.transform.Find("P3").gameObject.SetActive(false);
                currentSkin.transform.Find("P4").gameObject.SetActive(false);
                break;
            case 1:
                currentSkin.transform.Find("P1").gameObject.SetActive(false);
                currentSkin.transform.Find("P2").gameObject.SetActive(true);
                currentSkin.transform.Find("P3").gameObject.SetActive(false);
                currentSkin.transform.Find("P4").gameObject.SetActive(false);
                break;
            case 2:
                currentSkin.transform.Find("P1").gameObject.SetActive(false);
                currentSkin.transform.Find("P2").gameObject.SetActive(false);
                currentSkin.transform.Find("P3").gameObject.SetActive(true);
                currentSkin.transform.Find("P4").gameObject.SetActive(false);
                break;
            case 3:
                currentSkin.transform.Find("P1").gameObject.SetActive(false);
                currentSkin.transform.Find("P2").gameObject.SetActive(false);
                currentSkin.transform.Find("P3").gameObject.SetActive(false);
                currentSkin.transform.Find("P4").gameObject.SetActive(true);
                break;
        }
    }
}
