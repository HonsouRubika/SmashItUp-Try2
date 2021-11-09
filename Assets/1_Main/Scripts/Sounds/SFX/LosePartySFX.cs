using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LosePartySFX : MonoBehaviour
{
    [Space]
    [Header("FinalLose Sound")]
    public AudioClip finalLose;
    [Range(0f, 1f)] public float finalLoseVolume = 0.5f;

    public void PlayerFinalLose()
    {
        SoundManager.Instance.PlaySfx(finalLose, finalLoseVolume);
    }
}