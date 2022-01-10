using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
    [HideInInspector] public Animator playerAnimator;
    private PlayerController playerControllerScript;

    private void Start()
    {
        playerControllerScript = GetComponent<PlayerController>();
        SetCooldownTimeAnimation(1 / playerControllerScript.attackRate);
    }

    public void Idle(bool idle)
    {
        playerAnimator.SetBool("isIdle", idle);
    }

    public void Running(bool run)
    {
        playerAnimator.SetBool("isRunning", run);
    }

    public void Jumping(bool jump)
    {
        if(playerAnimator != null) playerAnimator.SetBool("isJumping", jump);
    }

    public void Expulsion(bool expulsed)
    {
        playerAnimator.SetBool("isExpulsing", expulsed);
    }

    public void Attack()
    {
        if(playerAnimator != null) playerAnimator.SetTrigger("isAttacking");
    }

    public void WallSlide(bool wallRide)
    {
        //on évite de changer la valeur si c'est la même => sinon ça reset l'animation à zero pour rien => créer une vibration
        if (playerAnimator.GetBool("isWallSliding") != wallRide) playerAnimator.SetBool("isWallSliding", wallRide);
    }

    public void Falling(bool fall)
    {
        playerAnimator.SetBool("isFalling", fall);
    }

    public void SetCooldownTimeAnimation(float cd)
    {
        playerAnimator.SetFloat("cooldownSpeed", cd);
    }
}
