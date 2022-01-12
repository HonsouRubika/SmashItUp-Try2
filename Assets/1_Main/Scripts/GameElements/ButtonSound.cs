using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSound : MonoBehaviour
{
    [Space]
    [Header("Button Sound")]
    public AudioClip button;
    [Range(0f, 1f)] public float buttonVolume = 0.5f;

    public void ButtonPressed()
    {
        SoundManager.Instance.PlaySfx(button, buttonVolume);
    }
}
