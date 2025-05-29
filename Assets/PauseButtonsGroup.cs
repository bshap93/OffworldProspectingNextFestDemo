using Domains.Scene.MainMenu;
using MoreMountains.Tools;
using UnityEngine;

public class PauseButtonsGroup : MonoBehaviour, MMEventListener<MainMenuEvent>
{
    private CanvasGroup pauseButtonsCanvasGroup;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        pauseButtonsCanvasGroup = GetComponent<CanvasGroup>();
        if (pauseButtonsCanvasGroup == null)
            Debug.LogError("PauseButtonsGroup: CanvasGroup component not found on this GameObject.");
    }

    public void OnEnable()
    {
        this.MMEventStartListening();
    }

    public void OnDisable()
    {
        this.MMEventStopListening();
    }

    public void OnMMEvent(MainMenuEvent eventType)
    {
        if (eventType.EventType == MainMenuEventType.SettingsOpenTriggered)
            HidePauseButtons();
        else if (eventType.EventType == MainMenuEventType.SettingsCloseTriggered) ShowPauseButtons();
    }

    public void ShowPauseButtons()
    {
        if (pauseButtonsCanvasGroup != null)
        {
            pauseButtonsCanvasGroup.alpha = 1f;
            pauseButtonsCanvasGroup.interactable = true;
            pauseButtonsCanvasGroup.blocksRaycasts = true;
        }
    }

    public void HidePauseButtons()
    {
        if (pauseButtonsCanvasGroup != null)
        {
            pauseButtonsCanvasGroup.alpha = 0f;
            pauseButtonsCanvasGroup.interactable = false;
            pauseButtonsCanvasGroup.blocksRaycasts = false;
        }
    }
}