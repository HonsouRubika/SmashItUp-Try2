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

    public void Open()
    {
        transitionAnimator.SetTrigger("opening");
    }

    public void Close()
    {
        transitionAnimator.SetTrigger("closing");
    }
}
