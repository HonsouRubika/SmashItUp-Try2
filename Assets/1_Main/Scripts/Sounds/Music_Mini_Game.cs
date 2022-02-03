using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Music_Mini_Game : MonoBehaviour
{
    [Space]
    [Header("Music Mini Game")]
    public AudioClip musicMiniGame;

    [Header("Timed music Mini game")]
    public AudioClip musicMiniGame30s;
    public AudioClip musicMiniGame45s;
    public AudioClip musicMiniGame60s;

    private AudioClip currentMusicMiniGame;
    [Range(0f, 1f)] public float musicMiniGameVolume = 0.3f;
    public bool loopMusic = true;

    private void Start()
    {
        if (musicMiniGame30s == null || musicMiniGame45s == null || musicMiniGame60s == null)
        {
            currentMusicMiniGame = musicMiniGame;
        }
        else
        {
            switch (GameManager.Instance.durationMiniGame)
            {
                case 30:
                    currentMusicMiniGame = musicMiniGame30s;
                    break;
                case 45:
                    currentMusicMiniGame = musicMiniGame45s;
                    break;
                case 60:
                    currentMusicMiniGame = musicMiniGame60s;
                    break;
            }
        }

        StartMusic();
    }

    public void StartMusic()
    {
        SoundManager.Instance.PlayMusic(currentMusicMiniGame, musicMiniGameVolume);
    }

    private void Update()
    {
        if (loopMusic && !SoundManager.Instance.musicSource.isPlaying && !GameManager.Instance.isPaused)
        {
            SoundManager.Instance.PlayMusic(currentMusicMiniGame, musicMiniGameVolume);
        }
    }
}
