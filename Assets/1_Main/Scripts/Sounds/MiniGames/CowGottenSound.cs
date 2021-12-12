using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CowGottenSound : MonoBehaviour
{
    [Space]
    [Header("CowGotten Sound")]
    public AudioClip cowGotten;
    [Range(0f, 1f)] public float cowGottenVolume = 0.5f;

    public void PlayerGotCow()
    {
        SoundManager.Instance.PlaySfx(cowGotten, cowGottenVolume);
    }
}

