using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject optionsMenu;
    public GameObject audioMenu;
    public GameObject videoMenu;

    [Space]
    public EventSystem eventSystemKeyboard;
    public EventSystem eventSystemController;

    [Header("Settings values")]
    [Range(0, 1)] public float globalVolume;
    [Range(0, 1)] public float musicVolume;
    [Range(0, 1)] public float sfxVolume;

    public bool fullScren;
    public int resolution;
    public int quality;

    private uint playerThatPausedID;

    public PauseSound PauseSoundScript;

    private void Start()
    {
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(false);
        audioMenu.SetActive(false);
        videoMenu.SetActive(false);

        LoadSettings();
    }

    public void GamePause(uint playerID, InputAction.CallbackContext context)
    {
        if (!GameManager.Instance.isPaused)
        {
            Pause();
            playerThatPausedID = playerID;


           


            if (context.control == Keyboard.current.escapeKey)
            {
                eventSystemKeyboard.gameObject.SetActive(true);
                eventSystemKeyboard.SetSelectedGameObject(null);
                eventSystemKeyboard.SetSelectedGameObject(pauseMenu.transform.GetChild(0).gameObject);
            }
            else
            {
                eventSystemController.gameObject.SetActive(true);
                eventSystemController.SetSelectedGameObject(null);
                eventSystemController.SetSelectedGameObject(pauseMenu.transform.GetChild(0).gameObject);
            }
        }
        else if (playerThatPausedID == playerID)
        {
            Resume();
        }
    }

    private void Pause()
    {
        pauseMenu.SetActive(true);

        PauseSoundScript.GameIsOnPause();

        Time.timeScale = 0f;
        GameManager.Instance.isPaused = true;
        SoundManager.Instance.PauseGame(true);
    }

    public void Resume()
    {
        eventSystemController.gameObject.SetActive(false);
        eventSystemKeyboard.gameObject.SetActive(false);
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(false);
        audioMenu.SetActive(false);
        videoMenu.SetActive(false);
        Time.timeScale = 1f;
        GameManager.Instance.isPaused = false;
        SoundManager.Instance.PauseGame(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetFullScreen(bool isFullScreen)
    {
        fullScren = isFullScreen;
        Screen.fullScreen = isFullScreen;
    }

    public void SetResolution(int resolutionIndex)
    {
        resolution = resolutionIndex;
        switch (resolutionIndex)
        {
            case 0:
                Screen.SetResolution(1920, 1080, Screen.fullScreen);
                break;
            case 1:
                Screen.SetResolution(3840, 2160, Screen.fullScreen);
                break;
            case 2:
                Screen.SetResolution(2560, 1440, Screen.fullScreen);
                break;
            case 3:
                Screen.SetResolution(1280, 720, Screen.fullScreen);
                break;
            case 4:
                Screen.SetResolution(640, 480, Screen.fullScreen);
                break;
        }
    }

    public void SetQuality(int qualityIndex)
    {
        quality = qualityIndex;
        switch (qualityIndex)
        {
            case 0:
                QualitySettings.SetQualityLevel(5);
                break;
            case 1:
                QualitySettings.SetQualityLevel(2);
                break;
            case 2:
                QualitySettings.SetQualityLevel(0);
                break;
        }
    }

    public void SetVolumeGlobal(float vol)
    {
        globalVolume = vol;
        SoundManager.Instance.globalDefaultVolume = vol;
    }

    public void SetVolumeSFX(float vol)
    {
        sfxVolume = vol;
        SoundManager.Instance.sfxDefaultVolume = vol;
    }

    public void SetVolumeMusic(float vol)
    {
        musicVolume = vol;
        SoundManager.Instance.musicDefaultVolume = vol;
    }

    public void SaveSettings()
    {
        SaveSystem.SaveSettings(this);
    }

    private void LoadSettings()
    {
        SettingsData data = SaveSystem.LoadPauseSettings();

        if (data != null)
        {
            globalVolume = data.globalVolume;
            musicVolume = data.musicVolume;
            sfxVolume = data.sfxVolume;

            fullScren = data.fullScren;
            resolution = data.resolution;
            quality = data.quality;

            UpdateSettingsValue();
        }
        else
        {
            globalVolume = audioMenu.transform.GetChild(0).GetComponent<Slider>().value;
            musicVolume = audioMenu.transform.GetChild(1).GetComponent<Slider>().value;
            sfxVolume = audioMenu.transform.GetChild(2).GetComponent<Slider>().value;
            fullScren = videoMenu.transform.GetChild(1).GetComponent<Toggle>().isOn;
            resolution = videoMenu.transform.GetChild(3).GetComponent<TMP_Dropdown>().value;
            quality = videoMenu.transform.GetChild(5).GetComponent<TMP_Dropdown>().value;
        }
    }

    private void UpdateSettingsValue()
    {
        audioMenu.transform.GetChild(0).GetComponent<Slider>().value = globalVolume;
        SetVolumeGlobal(globalVolume);

        audioMenu.transform.GetChild(1).GetComponent<Slider>().value = musicVolume;
        SetVolumeMusic(musicVolume);

        audioMenu.transform.GetChild(2).GetComponent<Slider>().value = sfxVolume;
        SetVolumeSFX(sfxVolume);

        videoMenu.transform.GetChild(1).GetComponent<Toggle>().isOn = fullScren;
        SetFullScreen(fullScren);

        videoMenu.transform.GetChild(3).GetComponent<TMP_Dropdown>().value = resolution;
        SetResolution(resolution);

        videoMenu.transform.GetChild(5).GetComponent<TMP_Dropdown>().value = quality;
        SetQuality(quality);
    }
}
