using System;
using MoreMountains.Tools;

namespace Domains.Scene.MainMenu
{
    [Serializable]
    public enum MainMenuEventType
    {
        NewGameTriggered,
        ContinueGameTriggered,
        SettingsOpenTriggered,
        QuitGameTriggered,

        // 
        NewGameAttempted,
        ExitDialog,
        SettingsCloseTriggered
    }

    public struct MainMenuEvent
    {
        private static MainMenuEvent _e;
        public MainMenuEventType EventType;


        public static void Trigger(MainMenuEventType eventType)
        {
            _e.EventType = eventType;


            MMEventManager.TriggerEvent(_e);
        }
    }
}