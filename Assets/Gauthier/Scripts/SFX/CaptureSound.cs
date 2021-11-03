using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureSound : MonoBehaviour
{
    [Space]
    [Header("Capture Sound")]
    public AudioClip capture;
    [Range(0f, 1f)] public float captureVolume = 0.5f;

    public void PlayerCapturing()
    {
        SoundManager.Instance.PlayZone(capture, captureVolume);
    }

    public void PlayerOutZone()
    {
        SoundManager.Instance.StopZone();

    }
}
