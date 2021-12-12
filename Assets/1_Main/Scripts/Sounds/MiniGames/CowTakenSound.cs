using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CowTakenSound : MonoBehaviour
{
    [Space]
    [Header("CowTaken Sound")]
    public AudioClip cowTaken;
    [Range(0f, 1f)] public float cowTakenVolume = 0.5f;

    public void PlayerTakeCow()
    {
        SoundManager.Instance.PlaySfx(cowTaken, cowTakenVolume);
    }
}
