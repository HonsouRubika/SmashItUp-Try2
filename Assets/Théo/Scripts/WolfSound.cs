using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class WolfSound : MonoBehaviour
{
    [Space]
    [Header("Wolf Sound")]
    public AudioClip wolfScream;
    [Range(0f, 1f)] public float screamVolume = 0.5f;

    public AudioClip wolfAttack;
    [Range(0f, 1f)] public float attackVolume = 0.5f;

    public void WolfScream()
    {
        SoundManager.Instance.PlaySfx(wolfScream, screamVolume);
    }

    public void WolfAttack()
    {
        SoundManager.Instance.PlaySfx(wolfAttack, attackVolume);
    }
}
