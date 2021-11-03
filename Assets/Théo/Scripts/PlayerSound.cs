using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    [Space]
    [Header("Player Sound")]
    public AudioClip[] hammerPouet;
    [Range(0f, 1f)] public float pouetVolume = 0.5f;

    public AudioClip hammerWoosh;
    [Range(0f, 1f)] public float wooshVolume = 0.5f;

    public AudioClip[] hammerHit;
    [Range(0f, 1f)] public float hitVolume = 0.5f;

    public AudioClip jump;
    [Range(0f, 1f)] public float jumpVolume = 0.5f;

    public AudioClip run;
    [Range(0f, 1f)] public float runVolume = 0.5f;

    public AudioClip ejection;
    [Range(0f, 1f)] public float ejectionVolume = 0.5f;
    public void HammerWoosh()
    {
        SoundManager.Instance.PlaySfx(hammerWoosh, wooshVolume);
    }

    public void HammerPouet()
    {
        SoundManager.Instance.PlayRandomSfx(hammerPouet, pouetVolume);
    }

    public void HammerHit()
    {
        SoundManager.Instance.PlayRandomSfx(hammerHit, hitVolume);
    }

    public void Jump()
    {
        SoundManager.Instance.PlaySfx(jump, jumpVolume);
    }

    public void Run()
    {
        SoundManager.Instance.PlaySfx(run, runVolume);
    }

    public void Ejection()
    {
        SoundManager.Instance.PlaySfx(ejection, ejectionVolume);
    }
}
