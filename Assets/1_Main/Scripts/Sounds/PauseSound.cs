using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseSound : MonoBehaviour
{
    [Space]
    [Header("Pause Sound")]
    public AudioClip pause;
    [Range(0f, 1f)] public float pauseVolume = 0.5f;

    public void GameIsOnPause()
    {
        SoundManager.Instance.PlayUI(pause, pauseVolume);
    }
}
