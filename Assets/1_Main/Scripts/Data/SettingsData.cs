using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SettingsData
{
    public float globalVolume;
    public float musicVolume;
    public float sfxVolume;

    public bool fullScren;
    public int resolution;
    public int quality;

    public SettingsData(PauseMenu pauseSettings)
    {
        globalVolume = pauseSettings.globalVolume;
        musicVolume = pauseSettings.musicVolume;
        sfxVolume = pauseSettings.sfxVolume;

        fullScren = pauseSettings.fullScren;
        resolution = pauseSettings.resolution;
        quality = pauseSettings.quality;
    }
}
