using Michsky.UI.Shift;
using MoreMountains.Tools;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    [Header("References")] [SerializeField]
    private QualityManager qualityManager;

    [Header("Sound References")] [SerializeField]
    private MMSoundManager soundManager;

    public static SettingsManager Instance { get; private set; }

    private void Awake()
    {
        // if (Instance != null && Instance != this)
        // {
        //     Destroy(gameObject);
        //     return;
        // }

        Instance = this;

        // Find QualityManager if not assigned
        if (qualityManager == null)
            qualityManager = FindFirstObjectByType<QualityManager>();

        // Find MMSoundManager if not assigned
        if (soundManager == null)
            soundManager = FindFirstObjectByType<MMSoundManager>();
    }

    private void Start()
    {
        // Load saved settings
        LoadSettings();
    }

    public void LoadSettings()
    {
        if (qualityManager == null)
        {
            Debug.LogError("No QualityManager found in the scene!");
            return;
        }

        // Load and apply quality settings
        var qualityLevel = PlayerPrefs.GetInt("QualityLevel", QualitySettings.GetQualityLevel());
        qualityManager.SetOverallQuality(qualityLevel);

        // Load and apply window mode
        var windowMode = PlayerPrefs.GetInt("WindowMode", 0); // 0 = Fullscreen, 1 = Borderless, 2 = Windowed

        switch (windowMode)
        {
            case 0:
                qualityManager.WindowFullscreen();
                break;
            case 1:
                qualityManager.WindowBorderless();
                break;
            case 2:
                qualityManager.WindowWindowed();
                break;
        }

        // Audio settings are loaded by MMSoundManager automatically
    }

    // Quality settings
    public void SetQualityLevel(int qualityIndex)
    {
        qualityManager.SetOverallQuality(qualityIndex);
        PlayerPrefs.SetInt("QualityLevel", qualityIndex);
        PlayerPrefs.Save();
    }

    // Y Inversion
    public void SetYInversion(bool isInverted)
    {
        // qualityManager.SetYInversion(isInverted);
        PlayerPrefs.SetInt("YInversion", isInverted ? 1 : 0);
        PlayerPrefs.Save();
    }

    // Resolution settings
    public void SetResolution(int resolutionIndex)
    {
        qualityManager.SetResolution(resolutionIndex);
        PlayerPrefs.SetInt("ResolutionIndex", resolutionIndex);
        PlayerPrefs.Save();
    }

    // Window mode specific methods
    public void SetWindowFullscreen()
    {
        qualityManager.WindowFullscreen();
        PlayerPrefs.SetInt("WindowMode", 0);
        PlayerPrefs.Save();
    }

    public void SetWindowBorderless()
    {
        qualityManager.WindowBorderless();
        PlayerPrefs.SetInt("WindowMode", 1);
        PlayerPrefs.Save();
    }

    public void SetWindowWindowed()
    {
        qualityManager.WindowWindowed();
        PlayerPrefs.SetInt("WindowMode", 2);
        PlayerPrefs.Save();
    }

    // Anti-aliasing
    public void SetAntiAliasing(int aaLevel)
    {
        qualityManager.AntiAlisasingSet(aaLevel);
        PlayerPrefs.SetInt("AntiAliasing", aaLevel);
        PlayerPrefs.Save();
    }

    // Shadow quality
    public void SetShadowQuality(int shadowLevel)
    {
        qualityManager.ShadowResolutionSet(shadowLevel);
        PlayerPrefs.SetInt("ShadowQuality", shadowLevel);
        PlayerPrefs.Save();
    }

    // Audio Settings using MMSoundManager
    public void SetMasterVolume(float volume)
    {
        if (soundManager != null) soundManager.SetVolumeMaster(volume);

        if (qualityManager != null && qualityManager.masterSlider != null)
        {
            qualityManager.masterSlider.mainSlider.value = volume;
            qualityManager.VolumeSetMaster(volume);
        }
    }

    public void SetMusicVolume(float volume)
    {
        if (soundManager != null) soundManager.SetVolumeMusic(volume);

        if (qualityManager != null && qualityManager.musicSlider != null)
        {
            qualityManager.musicSlider.mainSlider.value = volume;
            qualityManager.VolumeSetMusic(volume);
        }
    }

    public void SetSfxVolume(float volume)
    {
        if (soundManager != null) soundManager.SetVolumeSfx(volume);

        if (qualityManager != null && qualityManager.sfxSlider != null)
        {
            qualityManager.sfxSlider.mainSlider.value = volume;
            qualityManager.VolumeSetSFX(volume);
        }
    }
}