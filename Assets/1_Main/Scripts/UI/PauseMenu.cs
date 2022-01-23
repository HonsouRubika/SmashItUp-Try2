using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    public GameObject[] pauseMenu;
    public GameObject[] optionsMenu;
    public GameObject[] audioMenu;
    public GameObject[] videoMenu;

    public GameObject[] canvas; 

    [Space]
    public GameObject[] eventSystemPlayers;

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
        for (int i = 0; i < pauseMenu.Length; i++)
        {
            pauseMenu[i].SetActive(false);
            optionsMenu[i].SetActive(false);
            audioMenu[i].SetActive(false);
            videoMenu[i].SetActive(false);
        }

        LoadSettings();
    }

    public void GamePause(uint playerID, InputAction.CallbackContext context)
    {
        if (!GameManager.Instance.isPaused)
        {
            Pause();
            playerThatPausedID = playerID;
            /*
            for (int i = 0; i< GameManager.Instance.players.Length; i++)
            {
                GameManager.Instance.players[i].GetComponent<PlayerInput>().uiInputModule = eventSystemPlayers[i].GetComponent<>();
            }

            if (context.control == Keyboard.current.escapeKey)
            {
                eventSystemPlayers[playerThatPausedID].gameObject.SetActive(true);
                eventSystemPlayers[playerThatPausedID].SetSelectedGameObject(null);
                eventSystemPlayers[playerThatPausedID].SetSelectedGameObject(pauseMenu[playerThatPausedID].transform.GetChild(0).gameObject);
            }
            else
            {
                eventSystemPlayers[playerThatPausedID].gameObject.SetActive(true);
                eventSystemPlayers[playerThatPausedID].SetSelectedGameObject(null);
                eventSystemPlayers[playerThatPausedID].SetSelectedGameObject(pauseMenu[playerThatPausedID].transform.GetChild(0).gameObject);
            }
            */
        }
        else if (playerThatPausedID == playerID)
        {
            Resume();
        }
        
    }

    private void Pause()
    {
        pauseMenu[playerThatPausedID].SetActive(true);

        //link to camera
        foreach (Camera camera in FindObjectsOfType<Camera>())
        {
            if (camera.enabled == true) canvas[playerThatPausedID].GetComponent<Canvas>().worldCamera = camera;
        }

        //PauseSoundScript.GameIsOnPause();

        Time.timeScale = 0f;
        GameManager.Instance.isPaused = true;
        SoundManager.Instance.PauseGame(true);
    }

    public void Resume()
    {
        eventSystemPlayers[playerThatPausedID].gameObject.SetActive(false);
        eventSystemPlayers[playerThatPausedID].gameObject.SetActive(false);
        for (int i = 0; i < pauseMenu.Length; i++)
        {
            pauseMenu[i].SetActive(false);
            optionsMenu[i].SetActive(false);
            audioMenu[i].SetActive(false);
            videoMenu[i].SetActive(false);
        }
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
            globalVolume = audioMenu[playerThatPausedID].transform.GetChild(0).GetComponent<Slider>().value;
            musicVolume = audioMenu[playerThatPausedID].transform.GetChild(1).GetComponent<Slider>().value;
            sfxVolume = audioMenu[playerThatPausedID].transform.GetChild(2).GetComponent<Slider>().value;
            fullScren = videoMenu[playerThatPausedID].transform.GetChild(1).GetComponent<Toggle>().isOn;
            resolution = videoMenu[playerThatPausedID].transform.GetChild(3).GetComponent<TMP_Dropdown>().value;
            quality = videoMenu[playerThatPausedID].transform.GetChild(5).GetComponent<TMP_Dropdown>().value;
        }
    }

    private void UpdateSettingsValue()
    {
        audioMenu[playerThatPausedID].transform.GetChild(0).GetComponent<Slider>().value = globalVolume;
        SetVolumeGlobal(globalVolume);

        audioMenu[playerThatPausedID].transform.GetChild(1).GetComponent<Slider>().value = musicVolume;
        SetVolumeMusic(musicVolume);

        audioMenu[playerThatPausedID].transform.GetChild(2).GetComponent<Slider>().value = sfxVolume;
        SetVolumeSFX(sfxVolume);

        videoMenu[playerThatPausedID].transform.GetChild(1).GetComponent<Toggle>().isOn = fullScren;
        SetFullScreen(fullScren);

        videoMenu[playerThatPausedID].transform.GetChild(3).GetComponent<TMP_Dropdown>().value = resolution;
        SetResolution(resolution);

        videoMenu[playerThatPausedID].transform.GetChild(5).GetComponent<TMP_Dropdown>().value = quality;
        SetQuality(quality);
    }
}
