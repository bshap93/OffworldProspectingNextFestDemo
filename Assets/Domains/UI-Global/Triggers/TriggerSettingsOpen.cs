using Domains.Scene.MainMenu;
using UnityEngine;

public class TriggerSettingsOpen : MonoBehaviour
{
    public void TriggerSettingsOpenEvent()
    {
        // Trigger the event to open the settings menu
        MainMenuEvent.Trigger(MainMenuEventType.SettingsOpenTriggered);
    }
}