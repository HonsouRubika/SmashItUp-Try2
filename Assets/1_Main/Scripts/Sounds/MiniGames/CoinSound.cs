using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSound : MonoBehaviour
{
    [Space]
    [Header("Coin Sound")]
    public AudioClip coin;
    [Range(0f, 1f)] public float coinVolume = 0.5f;

    public void PlayerTakeCoin()
    {
        SoundManager.Instance.PlaySfx(coin, coinVolume);
    }
}
