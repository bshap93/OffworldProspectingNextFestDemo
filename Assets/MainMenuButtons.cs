using Domains.Scene.MainMenu;
using MoreMountains.Tools;
using UnityEngine;

public class MainMenuButtons : MonoBehaviour, MMEventListener<MainMenuEvent>
{
    private CanvasGroup mainMenuCanvasGroup;

    private void Awake()
    {
        mainMenuCanvasGroup = GetComponent<CanvasGroup>();
        if (mainMenuCanvasGroup == null)
            Debug.LogError("MainMenuButtons: No CanvasGroup found on this GameObject");
    }

    private void OnEnable()
    {
        this.MMEventStartListening();
    }

    private void OnDisable()
    {
        this.MMEventStopListening();
    }


    public void OnMMEvent(MainMenuEvent eventType)
    {
        switch (eventType.EventType)
        {
            case MainMenuEventType.SettingsOpenTriggered:
                CloseSettingsPanel();
                break;
            case MainMenuEventType.SettingsCloseTriggered:
                OpenSettingsPanel();
                break;
        }
    }

    private void OpenSettingsPanel()
    {
        // Logic to open settings panel
        mainMenuCanvasGroup.alpha = 1;
        mainMenuCanvasGroup.interactable = true;
        mainMenuCanvasGroup.blocksRaycasts = true;
    }

    private void CloseSettingsPanel()
    {
        // Logic to close settings panel
        mainMenuCanvasGroup.alpha = 0;
        mainMenuCanvasGroup.interactable = false;
        mainMenuCanvasGroup.blocksRaycasts = false;
    }
}