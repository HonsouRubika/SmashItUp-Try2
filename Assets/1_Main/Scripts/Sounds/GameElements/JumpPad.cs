using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPadSound : MonoBehaviour
{
    [Space]
    [Header("JumpPad Sound")]
    public AudioClip jumpPad;
    [Range(0f, 1f)] public float jumpPadVolume = 0.5f;

    public void PlayerUseJumpPad()
    {
        SoundManager.Instance.PlaySfx(jumpPad, jumpPadVolume);
    }
}

