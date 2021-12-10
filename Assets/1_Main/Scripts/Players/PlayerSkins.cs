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

    private void Start()
    {
        parent = GetComponentInParent<PlayerController>().transform;
        playerAnimScript = GetComponent<PlayerAnim>();
        playerControllerScript = parent.GetComponent<PlayerController>();

        currentSkin = baseSkin;
        playerAnimScript.playerAnimator = currentSkin.GetComponent<Animator>();
        playerControllerScript.playerAnimator = currentSkin.GetComponent<Animator>().transform;
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Test"))
        {
            if (Keyboard.current.enterKey.wasPressedThisFrame)
            {
                ChangeSkin();
            }

            if (Gamepad.current != null)
            {
                if (Gamepad.current.rightShoulder.wasPressedThisFrame)
                {
                    ChangeSkin();
                }
            }
        }
    }

    void ChangeSkin()
    {
        skinNumber++;

        if (skinNumber >= skins.Count)
        {
            skinNumber = 0;
        }
        
        Destroy(currentSkin);
        GameObject InstanceSkin = Instantiate(skins[skinNumber], skinPosition.position, Quaternion.identity, parent);

        currentSkin = InstanceSkin;
        playerAnimScript.playerAnimator = currentSkin.GetComponent<Animator>();
        playerControllerScript.playerAnimator = currentSkin.GetComponent<Animator>().transform;
    }
}
