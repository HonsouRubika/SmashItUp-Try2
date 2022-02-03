using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarnPointSound : MonoBehaviour
{
    [Space]
    [Header("EarnPoint Sound")]
    public AudioClip earnPoint;
    [Range(0f, 1f)] public float earnPointVolume = 0.5f;

    public void PlayerEarnPoint()
    {
        SoundManager.Instance.PlaySfx(earnPoint, earnPointVolume);
    }
}
