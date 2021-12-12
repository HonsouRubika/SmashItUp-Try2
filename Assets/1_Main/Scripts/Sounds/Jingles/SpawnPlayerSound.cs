using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlayerSound : MonoBehaviour
{
    [Space]
    [Header("SpawnPlayer Sound")]
    public AudioClip spawnPlayer;
    [Range(0f, 1f)] public float spawnPlayerVolume = 0.5f;

    public void PlayerSpawn()
    {
        SoundManager.Instance.PlaySfx(spawnPlayer, spawnPlayerVolume);
    }
}

