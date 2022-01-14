using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSound : MonoBehaviour
{
    [Space]
    [Header("Door Sound")]
    public AudioClip door;
    [Range(0f, 1f)] public float doorVolume = 0.5f;

    public void DoorOpening()
    {
        SoundManager.Instance.PlaySfx(door, doorVolume);
    }
}
