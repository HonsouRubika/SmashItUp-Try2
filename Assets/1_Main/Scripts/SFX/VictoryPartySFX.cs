using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryPartySFX : MonoBehaviour
{
    [Space]
    [Header("FinalVictory Sound")]
    public AudioClip finalVictory;
    [Range(0f, 1f)] public float finalVictoryVolume = 0.5f;

    public void PlayerFinalVictory()
    {
        SoundManager.Instance.PlaySfx(finalVictory, finalVictoryVolume);
    }
}
