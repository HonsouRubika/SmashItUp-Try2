using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEatenSound : MonoBehaviour
{
    [Space]
    [Header("PlayerEaten Sound")]
    public AudioClip playerEaten;
    [Range(0f, 1f)] public float playerEatenVolume = 0.5f;

    public void PlayerEatenByCarniPlant()
    {
        SoundManager.Instance.PlaySfx(playerEaten, playerEatenVolume);
    }
}

