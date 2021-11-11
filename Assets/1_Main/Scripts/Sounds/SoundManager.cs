using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// By Antoine LEROUX
/// Use this script to trigger a sound from any other script
/// </summary>

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    #region Inspector Settings
    [Space]
    [Header("Global")]
    [Range(0f, 1f)] public float globalDefaultVolume = 0.5f;

    [Space]
    [Header("Musics")]
    [Range(0f, 1f)] public float musicDefaultVolume = 0.5f;

    [Space]
    [Header("SFX")]
    [Range(0f, 1f)] public float sfxDefaultVolume = 0.5f;

    [Space]
    [Header("Voice")]
    [Range(0f, 1f)] public float voiceDefaultVolume = 0.5f;

    [Space]
    [Header("References")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource voiceSource;
    public AudioSource zoneSource;

    #endregion

    void Awake()
    {
        #region Make Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        #endregion
    }

    public void SetVolumeGlobal(float vol)
    {
        globalDefaultVolume = vol;
    }

    public void SetVolumeSFX(float vol)
    {
        sfxDefaultVolume = vol;
    }

    public void SetVolumeMusic(float vol)
    {
        musicDefaultVolume = vol;
    }

    public void SetVolumeVoice(float vol)
    {
        voiceDefaultVolume = vol;
    }

    // Start playing a given music.
    public void PlayMusic(AudioClip music, float volume = 1f)
    {
        musicSource.clip = music;
        musicSource.volume = (musicDefaultVolume * volume) * globalDefaultVolume;
        musicSource.Play();

        return;
    }

    // Plays a given sfx. Specific volume and pitch can be specified in parameters.
    public void PlaySfx(AudioClip sfx, float volume = 1f, float pitch = 1f)
    {
        sfxSource.pitch = pitch;
        sfxSource.PlayOneShot(sfx, (sfxDefaultVolume * volume) * globalDefaultVolume);

        sfxSource.pitch = 1;

        return;
    }

    // Plays a random sfx from a list.
    public void PlayRandomSfx(AudioClip[] sfxRandom, float volume = 1f, float pitch = 1f)
    {
        int randomIndex = Random.Range(0, sfxRandom.Length);

        sfxSource.pitch = pitch;
        sfxSource.PlayOneShot(sfxRandom[randomIndex], (sfxDefaultVolume * volume) * globalDefaultVolume);

        sfxSource.pitch = 1;

        return;
    }

    // Plays a voice sound.
    public void PlayVoice(AudioClip voice, float volume = 1f, float pitch = 1f)
    {
        voiceSource.Stop();

        voiceSource.pitch = pitch;
        if (!voiceSource.isPlaying)
        {
            voiceSource.PlayOneShot(voice, (voiceDefaultVolume * volume) * globalDefaultVolume);
        }

        voiceSource.pitch = 1;

        return;
    }

    public void PlayZone(AudioClip sfx, float volume = 1f, float pitch = 1f)
    {
        zoneSource.pitch = pitch;
        zoneSource.PlayOneShot(sfx, (sfxDefaultVolume * volume) * globalDefaultVolume);

        zoneSource.pitch = 1;

        return;
    }
    public void StopZone()
    {
        zoneSource.Stop();

        return;
    }
}
