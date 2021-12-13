using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenAppearSound : MonoBehaviour
{
    [Space]
    [Header("ScreenAppear Sound")]
    public AudioClip screenAppear;
    [Range(0f, 1f)] public float screenAppearVolume = 0.5f;

    public void ScreenAppearing()
    {
        SoundManager.Instance.PlaySfx(screenAppear, screenAppearVolume);
    }
}
