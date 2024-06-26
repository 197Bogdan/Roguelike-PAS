using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{

    List<Resolution> filteredResolutions;
    Resolution[] resolutions;
    public TMPro.TMP_Dropdown resolutionDropdown;
    int defaultResolutionIndex;
    double defaultRefreshRate;


    void Start()
    {
        defaultRefreshRate = Screen.currentResolution.refreshRateRatio.value;
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

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

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = defaultResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        SetVSync(false);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = filteredResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        Debug.Log("Resolution: " + resolution.width + " x " + resolution.height);
    }

    public void SetVSync(bool isOn)
    {
        if(isOn)
            QualitySettings.vSyncCount = 1;
        else
            QualitySettings.vSyncCount = 0;
        Debug.Log("VSync: " + isOn);
    }
}
