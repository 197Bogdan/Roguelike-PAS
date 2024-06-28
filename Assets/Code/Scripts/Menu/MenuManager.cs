using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{

    UserSettings userSettings;

    public TMPro.TMP_Dropdown resolutionDropdown;
    public Slider volumeSlider;
    public Toggle vsyncToggle;
    public GameObject pauseMenu;
    public bool isPaused = false;

    List<Resolution> filteredResolutions;
    int defaultResolutionIndex;

    void Start()
    {
        CreateResolutionDropdown();
        userSettings = new UserSettings();
        LoadSettings();     // load settings if they exist
        SetSettings();      
    }

    public void CreateResolutionDropdown()
    {
        double defaultRefreshRate = Screen.currentResolution.refreshRateRatio.value;
        Resolution[] resolutions = Screen.resolutions;
        filteredResolutions = new List<Resolution>();

        foreach(Resolution resolution in resolutions)
        {
            if(resolution.refreshRateRatio.value == defaultRefreshRate)
                filteredResolutions.Add(resolution);
        }

        List<string> options = new List<string>();
        for(int i = 0; i < filteredResolutions.Count; i++)
        {
            string option = filteredResolutions[i].width + " x " + filteredResolutions[i].height;
            options.Add(option);

            if(filteredResolutions[i].width == Screen.currentResolution.width && filteredResolutions[i].height == Screen.currentResolution.height)
                defaultResolutionIndex = i;
        }

        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(options);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Game");
        isPaused = false;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("Menu");
        isPaused = false;
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        userSettings.volume = volume;
        SaveSettings();
    }

    public void SetResolution(int resolutionIndex)
    {
        if(resolutionIndex == -1)
            resolutionIndex = defaultResolutionIndex;

        Resolution resolution = filteredResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        userSettings.resolutionIndex = resolutionIndex;
        SaveSettings();
    }

    public void SetVSync(bool isVsync)
    {
        if(isVsync)
            QualitySettings.vSyncCount = 1;
        else
            QualitySettings.vSyncCount = 0;
        userSettings.isVsync = isVsync;
        SaveSettings();
    }

    public void SaveSettings()
    {
        string json = JsonUtility.ToJson(userSettings);
        File.WriteAllText(Application.persistentDataPath + "/settings.json", json);
    }

    public void LoadSettings()
    {
        string path = Application.persistentDataPath + "/settings.json";
        if(File.Exists(path))
        {
            string json = File.ReadAllText(path);
            userSettings = JsonUtility.FromJson<UserSettings>(json);
        }
    }

    public void SetSettings()
    {
        volumeSlider.value = userSettings.volume;
        SetVolume(userSettings.volume);
        vsyncToggle.isOn = userSettings.isVsync;
        SetVSync(userSettings.isVsync);

        resolutionDropdown.value = defaultResolutionIndex;  // indirectly calls SetResolution
        resolutionDropdown.RefreshShownValue();
    }

    public void PauseGame()
    {
        if(isPaused)
            ResumeGame();
        else
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0;
            isPaused = true;
        }
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
        isPaused = false;
    }

    public bool IsPaused()
    {
        return isPaused;
    }

    public void SaveGame()
    {
        // Save game
    }
}
