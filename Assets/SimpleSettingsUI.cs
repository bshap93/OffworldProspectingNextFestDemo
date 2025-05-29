using System.Collections.Generic;
using Michsky.UI.Shift;
using MoreMountains.Tools;
using TMPro;
using UnityEngine;

public class SimpleSettingsUI : MonoBehaviour
{
    [Header("References")] [SerializeField]
    private QualityManager qualityManager;

    [SerializeField] private SettingsManager settingsManager;
    [SerializeField] private MMSoundManager soundManager;

    [Header("UI Elements")] [SerializeField]
    private TMP_Dropdown qualityDropdown;

    [SerializeField] private HorizontalSelector windowModeSelector;
    [SerializeField] private HorizontalSelector antiAliasingSelector;
    [SerializeField] private HorizontalSelector shadowQualitySelector;

    private void Start()
    {
        // Find managers if not assigned
        if (qualityManager == null)
            qualityManager = FindFirstObjectByType<QualityManager>();

        if (soundManager == null)
            soundManager = FindFirstObjectByType<MMSoundManager>();

        if (settingsManager == null)
            settingsManager = SettingsManager.Instance;

        if (qualityManager == null || settingsManager == null)
        {
            Debug.LogError("Required manager references are missing!");
            return;
        }


        // Setup UI elements
        SetupQualityDropdown();
        SetupWindowModeSelector();
        SetupAntiAliasingSelector();
        SetupShadowQualitySelector();

        // Connect audio sliders - these need both QualityManager and MMSoundManager
        ConnectAudioSliders();
    }

    private void ConnectAudioSliders()
    {
        // Connect Master Volume Slider
        if (qualityManager.masterSlider != null && qualityManager.masterSlider.mainSlider != null)
        {
            qualityManager.masterSlider.mainSlider.onValueChanged.RemoveAllListeners();
            qualityManager.masterSlider.mainSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        }

        // Connect Music Volume Slider
        if (qualityManager.musicSlider != null && qualityManager.musicSlider.mainSlider != null)
        {
            qualityManager.musicSlider.mainSlider.onValueChanged.RemoveAllListeners();
            qualityManager.musicSlider.mainSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        }

        // Connect SFX Volume Slider
        if (qualityManager.sfxSlider != null && qualityManager.sfxSlider.mainSlider != null)
        {
            qualityManager.sfxSlider.mainSlider.onValueChanged.RemoveAllListeners();
            qualityManager.sfxSlider.mainSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
        }
    }

    private void OnMasterVolumeChanged(float volume)
    {
        if (settingsManager != null)
            settingsManager.SetMasterVolume(volume);
    }

    private void OnMusicVolumeChanged(float volume)
    {
        if (settingsManager != null)
            settingsManager.SetMusicVolume(volume);
    }

    private void OnSfxVolumeChanged(float volume)
    {
        if (settingsManager != null)
            settingsManager.SetSfxVolume(volume);
    }


    private void SetupQualityDropdown()
    {
        if (qualityDropdown == null) return;

        qualityDropdown.ClearOptions();

        var options = new List<string>();
        var qualityNames = QualitySettings.names;

        for (var i = 0; i < qualityNames.Length; i++) options.Add(qualityNames[i]);

        qualityDropdown.AddOptions(options);
        qualityDropdown.value = PlayerPrefs.GetInt("QualityLevel", QualitySettings.GetQualityLevel());
        qualityDropdown.RefreshShownValue();

        // Add listener
        qualityDropdown.onValueChanged.AddListener(OnQualityChanged);
    }

    private void SetupWindowModeSelector()
    {
        if (windowModeSelector == null) return;

        // Clear existing items
        windowModeSelector.itemList.Clear();

        // Create items with their actions
        windowModeSelector.CreateNewItem("Fullscreen");
        windowModeSelector.itemList[0].onValueChanged.AddListener(() => { settingsManager.SetWindowFullscreen(); });

        windowModeSelector.CreateNewItem("Windowed");
        windowModeSelector.itemList[1].onValueChanged.AddListener(() => { settingsManager.SetWindowWindowed(); });

        // Set current value
        windowModeSelector.index = PlayerPrefs.GetInt("WindowMode", 0);
        windowModeSelector.UpdateUI();
    }

    private void SetupAntiAliasingSelector()
    {
        if (antiAliasingSelector == null) return;

        // Clear existing items
        antiAliasingSelector.itemList.Clear();

        // Create items with their actions
        antiAliasingSelector.CreateNewItem("Off");
        antiAliasingSelector.itemList[0].onValueChanged.AddListener(() => { settingsManager.SetAntiAliasing(0); });

        antiAliasingSelector.CreateNewItem("2x");
        antiAliasingSelector.itemList[1].onValueChanged.AddListener(() => { settingsManager.SetAntiAliasing(2); });

        antiAliasingSelector.CreateNewItem("4x");
        antiAliasingSelector.itemList[2].onValueChanged.AddListener(() => { settingsManager.SetAntiAliasing(4); });

        antiAliasingSelector.CreateNewItem("8x");
        antiAliasingSelector.itemList[3].onValueChanged.AddListener(() => { settingsManager.SetAntiAliasing(8); });

        // Set current value
        var aaValue = QualitySettings.antiAliasing;
        var indexValue = 0;

        switch (aaValue)
        {
            case 0:
                indexValue = 0;
                break;
            case 2:
                indexValue = 1;
                break;
            case 4:
                indexValue = 2;
                break;
            case 8:
                indexValue = 3;
                break;
        }

        antiAliasingSelector.index = indexValue;
        antiAliasingSelector.UpdateUI();
    }

    private void SetupShadowQualitySelector()
    {
        if (shadowQualitySelector == null) return;

        // Clear existing items
        shadowQualitySelector.itemList.Clear();

        // Create items with their actions
        shadowQualitySelector.CreateNewItem("Off");
        shadowQualitySelector.itemList[0].onValueChanged.AddListener(() => { settingsManager.SetShadowQuality(0); });

        shadowQualitySelector.CreateNewItem("Low");
        shadowQualitySelector.itemList[1].onValueChanged.AddListener(() => { settingsManager.SetShadowQuality(1); });

        shadowQualitySelector.CreateNewItem("Medium");
        shadowQualitySelector.itemList[2].onValueChanged.AddListener(() => { settingsManager.SetShadowQuality(2); });

        shadowQualitySelector.CreateNewItem("High");
        shadowQualitySelector.itemList[3].onValueChanged.AddListener(() => { settingsManager.SetShadowQuality(3); });

        // Set current value based on shadow quality (approximate)
        var shadowValue = (int)QualitySettings.shadowResolution;
        shadowQualitySelector.index = shadowValue;
        shadowQualitySelector.UpdateUI();
    }

    // Quality dropdown event handler
    private void OnQualityChanged(int index)
    {
        settingsManager.SetQualityLevel(index);
    }

    // Reset to defaults button
    public void ResetToDefaults()
    {
        // Reset quality
        if (qualityDropdown != null)
        {
            qualityDropdown.value = Mathf.Min(3, qualityDropdown.options.Count - 1);
            OnQualityChanged(qualityDropdown.value);
        }

        // Reset audio sliders
        if (qualityManager.masterSlider != null && qualityManager.masterSlider.mainSlider != null)
        {
            qualityManager.masterSlider.mainSlider.value = 0.75f;
            OnMasterVolumeChanged(0.75f);
        }

        if (qualityManager.musicSlider != null && qualityManager.musicSlider.mainSlider != null)
        {
            qualityManager.musicSlider.mainSlider.value = 0.75f;
            OnMusicVolumeChanged(0.75f);
        }

        if (qualityManager.sfxSlider != null && qualityManager.sfxSlider.mainSlider != null)
        {
            qualityManager.sfxSlider.mainSlider.value = 0.75f;
            OnSfxVolumeChanged(0.75f);
        }


        // Reset window mode to fullscreen
        if (windowModeSelector != null)
        {
            windowModeSelector.index = 0;
            windowModeSelector.UpdateUI();
            settingsManager.SetWindowFullscreen();
        }

        // Reset AA
        if (antiAliasingSelector != null)
        {
            antiAliasingSelector.index = 1; // 2x
            antiAliasingSelector.UpdateUI();
            settingsManager.SetAntiAliasing(2);
        }

        // Reset shadow quality
        if (shadowQualitySelector != null)
        {
            shadowQualitySelector.index = 2; // Medium
            shadowQualitySelector.UpdateUI();
            settingsManager.SetShadowQuality(2);
        }

        // Save all settings
        PlayerPrefs.Save();

        // Save MM Sound Manager settings explicitly
        if (soundManager != null) MMSoundManagerEvent.Trigger(MMSoundManagerEventTypes.SaveSettings);
    }

    // Close the settings menu using your existing system
    public void CloseSettingsMenu()
    {
        // Find and use your existing settings panel close method
        var settingsPanel = FindFirstObjectByType<SettingsPanelsMainMenu>();
        if (settingsPanel != null) settingsPanel.TriggerClose();
    }
}