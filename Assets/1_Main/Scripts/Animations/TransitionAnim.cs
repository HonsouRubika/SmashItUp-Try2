using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionAnim : MonoBehaviour
{
    [HideInInspector] public Animator transitionAnimator;

    private void Awake()
    {
        transitionAnimator = GetComponent<Animator>();
    }

    public void OpenRed()
    {
        transitionAnimator.SetTrigger("opening");
    }

    public void CloseRed()
    {
        transitionAnimator.SetTrigger("closing");
    }

    public void OpenYellow()
    {
        transitionAnimator.SetTrigger("openYellow");
    }

    public void CloseYellow()
    {
        transitionAnimator.SetTrigger("closeYellow");
    }
    public void OpenBlue()
    {
        transitionAnimator.SetTrigger("openBlue");
    }

    public void CloseBlue()
    {
        transitionAnimator.SetTrigger("closeBlue");
    }
}
