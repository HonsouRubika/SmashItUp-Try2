using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Music_Mini_Game : MonoBehaviour
{
    [Space]
    [Header("Music Mini Game")]
    public AudioClip musicMiniGame;
    [Range(0f, 1f)] public float musicMiniGameVolume = 0.5f;

    private void Start()
    {
        SoundManager.Instance.PlayMusic(musicMiniGame, musicMiniGameVolume);
    }
}
