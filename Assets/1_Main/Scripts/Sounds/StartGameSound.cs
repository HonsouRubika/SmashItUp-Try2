using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGameSound : MonoBehaviour
{
    [Space]
    [Header("StartGame Sound")]
    public AudioClip startGame;
    [Range(0f, 1f)] public float startGameVolume = 0.5f;

    public void GameStarting()
    {
        SoundManager.Instance.PlaySfx(startGame, startGameVolume);
    }
}
