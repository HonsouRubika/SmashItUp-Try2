using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinnerSound : MonoBehaviour
{
    [Space]
    [Header("Spinner Sound")]
    public AudioClip spinner;
    [Range(0f, 1f)] public float spinnerVolume = 0.5f;

    public void ScoreSpinning()
    {
        SoundManager.Instance.PlaySfx(spinner, spinnerVolume);
    }

    [Space]
    [Header("EndSpinning Sound")]
    public AudioClip endSpinning;
    [Range(0f, 1f)] public float endSpinningVolume = 0.5f;
    public void EndSpinning()
    {
        SoundManager.Instance.PlaySfx(endSpinning, endSpinningVolume);
    }
}
