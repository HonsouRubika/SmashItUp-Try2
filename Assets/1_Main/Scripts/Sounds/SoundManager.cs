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
    public AudioSource footStepsSource;

    private bool fadeOut = false;

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

    #region PauseSettings
    public void PauseGame(bool isPause)
    {
        if (!isPause)
        {
            musicSource.UnPause();
            sfxSource.volume = 1f;
            zoneSource.volume = 1f;
            footStepsSource.volume = 1f;
            voiceSource.volume = 1f;
        }
        else
        {
            musicSource.Pause();
            sfxSource.volume = 0f;
            zoneSource.volume = 0f;
            footStepsSource.volume = 0f;
            voiceSource.volume = 0f;
        }
    }
    #endregion

    #region SetVolume
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
    #endregion

    #region FunctionsPlaySound
    // Start playing a given music.
    public void PlayMusic(AudioClip music, float volume = 1f)
    {
        musicSource.Stop();
        musicSource.clip = music;
        musicSource.volume = (musicDefaultVolume * volume) * globalDefaultVolume;
        musicSource.Play();

        return;
    }

    public void StopMusic()
    {
        musicSource.Stop();

        return;
    }

    // Fade in a given music.
    public void FadeInMusic(AudioClip music, float volume = 1f, float fadeTime = 1f)
    {
        StopAllCoroutines();
        fadeOut = false;

        musicSource.Stop();
        StartCoroutine(FadeIn(music, volume, fadeTime));

        return;
    }

    public void FadeOutMusic(float fadeTime = 1f)
    {
        StopAllCoroutines();
        fadeOut = false;

        StartCoroutine(FadeOut(fadeTime));

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

    public void PlayFootSteps(AudioClip footSteps, float volume = 1f, float pitch = 1f)
    {
        footStepsSource.pitch = pitch;
        if (!footStepsSource.isPlaying)
        {
            footStepsSource.PlayOneShot(footSteps, (sfxDefaultVolume * volume) * globalDefaultVolume);
        }

        footStepsSource.pitch = 1;

        return;
    }
    #endregion

    #region Coroutine
    IEnumerator FadeIn(AudioClip music, float volume, float fadeTime)
    {
        PlayMusic(music, volume);
        musicSource.volume = 0f;

        float currentTime = 0;
        while (currentTime < fadeTime)
        {
            currentTime += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(0, (musicDefaultVolume * volume) * globalDefaultVolume, currentTime / fadeTime);

            yield return null;
        }
    }

    IEnumerator FadeOut(float fadeTime)
    {
        fadeOut = true;
        float currentVolume = musicSource.volume;
        float currentTime = 0;
        while (currentTime < fadeTime)
        {
            currentTime += Time.unscaledDeltaTime;
            musicSource.volume = Mathf.Lerp(currentVolume, 0, currentTime / fadeTime);

            yield return null;
        }
        musicSource.Stop();
        fadeOut = false;
    }
    #endregion
}
