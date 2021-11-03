using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagSound : MonoBehaviour
{
    [Space]
    [Header("Flag Sound")]
    public AudioClip flag;
    [Range(0f, 1f)] public float flagVolume = 0.5f;

    public void PlayerTakeFlag()
    {
        SoundManager.Instance.PlaySfx(flag, flagVolume);
    }
}
