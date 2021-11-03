using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryMiniGamesSound : MonoBehaviour
{
    [Space]
    [Header("Victory Sound")]
    public AudioClip victory;
    [Range(0f, 1f)] public float victoryVolume = 0.5f;

    public void PlayerVictory()
    {
        SoundManager.Instance.PlaySfx(victory, victoryVolume);
    }
}
