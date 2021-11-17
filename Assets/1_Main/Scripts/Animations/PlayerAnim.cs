using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
    [HideInInspector] public Animator playerAnimator;

    private void Start()
    {
        playerAnimator = GetComponent<Animator>();
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
        playerAnimator.SetBool("isJumping", jump);
    }

    public void Expulsion(bool expulsed)
    {
        playerAnimator.SetBool("isExpulsing", expulsed);
    }

    public void Attack()
    {
        playerAnimator.SetTrigger("isAttacking");
    }

    public void WallSlide(bool wallRide)
    {
        playerAnimator.SetBool("isWallSliding", wallRide);
    }

    public void Falling(bool fall)
    {
        playerAnimator.SetBool("isFalling", fall);
    }
}
