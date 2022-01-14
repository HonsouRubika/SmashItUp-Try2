using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameRuleBumper : MonoBehaviour
{
    private bool isActive = false; //pour animation
    public RULE bumperRule = RULE.nbManches;

    [Header("Values range")]
    public int[] rangeNbManches = { 5, 7, 10 };
    public int[] rangeDureeManche = { 30, 45, 60 };
    private int iterator = 0;

    private Animator buttonAnimator;
    private TextMeshPro textRules;

    private int playerIDWhoTriggeredButton = 0;

    public ParticleSystem Button;

    public ButtonSound ButtonSoundScript;
    
private void Start()
{
    buttonAnimator = GetComponentInParent<Animator>();
    textRules = transform.parent.GetComponentInChildren<TextMeshPro>();

    switch (bumperRule)
    {
        case RULE.nbManches:
            textRules.text = rangeNbManches[iterator].ToString();
            break;
        case RULE.dureeManche:
            textRules.text = rangeDureeManche[iterator].ToString();
            break;
    }
}

    //Test FX 
    void CreateButtonDust()
    {
        Button.Play();
    }


    private void OnTriggerEnter2D(Collider2D collision)
{
    //Check if Player is jumping on the button
    if (collision.CompareTag("Player") && collision.GetComponent<PlayerController>().jumpState == JumpState.Falling && !isActive)
    {
        playerIDWhoTriggeredButton = (int)collision.GetComponent<PlayerController>().playerID;

        isActive = true; //pour animation

        ChangeGameRules();
        TriggerButtonAnim(true);

        CreateButtonDust();

        ButtonSoundScript.ButtonPressed();

    }
}

private void ChangeGameRules()
{
    iterator++;

    switch (bumperRule)
    {
        case RULE.nbManches:
            if (iterator == rangeNbManches.Length) iterator = 0;
            GameManager.Instance._nbManches = rangeNbManches[iterator];
            textRules.text = rangeNbManches[iterator].ToString();
            break;
        case RULE.dureeManche:
            if (iterator == rangeDureeManche.Length) iterator = 0;
            GameManager.Instance.durationMiniGame = rangeDureeManche[iterator];
            textRules.text = rangeDureeManche[iterator].ToString();
            break;
    }
}

private void TriggerButtonAnim(bool press)
{
    buttonAnimator.SetBool("ButtonPress", press);
}

private void OnTriggerExit2D(Collider2D collision)
{
    if (collision.CompareTag("Player") && collision.GetComponent<PlayerController>().playerID == playerIDWhoTriggeredButton)
    {
        isActive = false;
        TriggerButtonAnim(false);
    }
}

public enum RULE
{
    nbManches,
    dureeManche
}

}
