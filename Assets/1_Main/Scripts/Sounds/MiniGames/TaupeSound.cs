using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaupeSound : MonoBehaviour
{
    [Space]
    [Header("TaupeSpawn Sound")]
    public AudioClip taupeSpawn;
    [Range(0f, 1f)] public float taupeSpawnVolume = 0.5f;

    public void TaupeSpawning()
    {
        SoundManager.Instance.PlaySfx(taupeSpawn, taupeSpawnVolume);
    }

    [Space]
    [Header("TaupeHit Sound")]
    public AudioClip taupeHit;
    [Range(0f, 1f)] public float taupeHitVolume = 0.5f;

    public void TaupeIsHit()
    {
        SoundManager.Instance.PlaySfx(taupeHit, taupeHitVolume);
    }

}
