using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountDownSFX : MonoBehaviour
{
    [Space]
    [Header("CountDown Sound")]
    public AudioClip countDown;
    [Range(0f, 1f)] public float countDownVolume = 0.5f;

    public void PlayerInLastSeconds()
    {
        SoundManager.Instance.PlaySfx(countDown, countDownVolume);
    }
}