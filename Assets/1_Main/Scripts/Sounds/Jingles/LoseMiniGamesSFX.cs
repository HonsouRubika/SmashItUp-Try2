using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseMiniGamesSFX : MonoBehaviour
{
    [Space]
    [Header("Lose Sound")]
    public AudioClip lose;
    [Range(0f, 1f)] public float loseVolume = 0.5f;

    public void PlayerLost()
    {
        SoundManager.Instance.PlaySfx(lose, loseVolume);
    }
}