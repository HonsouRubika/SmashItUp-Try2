using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructSound : MonoBehaviour
{
    [Space]
    [Header("Destruct Sound")]
    public AudioClip destruct;
    [Range(0f, 1f)] public float destructVolume = 0.5f;

    public void PlayerDestroy()
    {
        SoundManager.Instance.PlaySfx(destruct, destructVolume);
    }
}
