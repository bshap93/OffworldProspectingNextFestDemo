using Domains.UI_Global.Events;
using MoreMountains.Tools;
using UnityEngine;

public class SettingsPanelsMainMenu : MonoBehaviour, MMEventListener<UIEvent>
{
    private CanvasGroup settingsPanelCanvasGroup;

    private void Awake()
    {
        settingsPanelCanvasGroup = GetComponent<CanvasGroup>();
        if (settingsPanelCanvasGroup == null)
            Debug.LogError("SettingsPanelsMainMenu: No CanvasGroup found on this GameObject");
    }

    private void OnEnable()
    {
        this.MMEventStartListening();
    }

    private void OnDisable()
    {
        this.MMEventStopListening();
    }

    public void OnMMEvent(UIEvent eventType)
    {
        switch (eventType.EventType)
        {
            case UIEventType.OpenSettings:
                OpenSettingsPanel();
                break;
            case UIEventType.CloseSettings:
                CloseSettingsPanel();
                break;
        }
    }

    public void TriggerClose()
    {
        UIEvent.Trigger(UIEventType.CloseSettings);
    }

    public void TriggerOpen()
    {
        UIEvent.Trigger(UIEventType.OpenSettings);
    }


    private void OpenSettingsPanel()
    {
        // Logic to open settings panel
        settingsPanelCanvasGroup.alpha = 1;
        settingsPanelCanvasGroup.interactable = true;
        settingsPanelCanvasGroup.blocksRaycasts = true;
    }

    private void CloseSettingsPanel()
    {
        // Logic to close settings panel
        settingsPanelCanvasGroup.alpha = 0;
        settingsPanelCanvasGroup.interactable = false;
        settingsPanelCanvasGroup.blocksRaycasts = false;
    }
}