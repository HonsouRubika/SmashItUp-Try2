using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerSkins : MonoBehaviour
{
    public GameObject baseSkin;
    public Transform skinPosition; 

    [Header("Current Skin")]
    public GameObject currentSkin;

    [Header("All skins")]
    public List<GameObject> skins;

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
    }

    public void ChangeSkin(InputAction.CallbackContext context)
    {
        //https://issuetracker.unity3d.com/issues/input-system-unity-events-called-twice-when-using-player-input-manager-and-player-input
        //je dois utiliser un timer parce que le new input system est buggé => la fonction est call deux fois

        if (context.started && Time.time >= changerSkinTimerActu)
        {
            changerSkinTimerActu = Time.time + changerSkinTimer;


            Debug.Log("fnct called");
            skinNumber++;

            if (skinNumber >= skins.Count)
            {
                skinNumber = 0;
            }

            //on supprime le skin actuel
            Destroy(currentSkin);

            //on ajoute le nouveau skin
            currentSkin = Instantiate(skins[skinNumber], skinPosition.position, Quaternion.identity, parent);

            if (currentSkin.GetComponent<Animator>() == null) Debug.Log("error");
            ///TODO : add un animator au deuxième skin
            playerAnimScript.playerAnimator = currentSkin.GetComponent<Animator>();
            playerControllerScript.playerAnimator = currentSkin.GetComponent<Animator>().transform;

        }
    }
}
