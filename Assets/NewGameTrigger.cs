using Domains.Scene.MainMenu;
using UnityEngine;

public class NewGameTrigger : MonoBehaviour
{
    public void TriggerNewGame()
    {
        // Trigger the new game event
        MainMenuEvent.Trigger(MainMenuEventType.NewGameTriggered);
    }
}