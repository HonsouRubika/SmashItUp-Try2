using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class TeleporterSound : MonoBehaviour
{
    [Space]
    [Header("Teleport Sound")]
    public AudioClip teleport;
    [Range(0f, 1f)] public float teleportVolume = 0.5f;

    public void PlayerTeleported()
    {
        SoundManager.Instance.PlaySfx(teleport, teleportVolume);
    }
}
