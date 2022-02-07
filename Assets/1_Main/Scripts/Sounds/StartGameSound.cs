using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGameSound : MonoBehaviour
{
    [Space]
    [Header("StartGame Sound")]
    public AudioClip startGame;
    [Range(0f, 1f)] public float startGameVolume = 0.5f;

    public void GameStarting(bool enable)
    {
        switch (enable)
        {
            case true:
                SoundManager.Instance.PlayJingleStart(startGame, startGameVolume);
                break;
            case false:
                SoundManager.Instance.StopJingleStart();
                break;
        } 
    }
}
