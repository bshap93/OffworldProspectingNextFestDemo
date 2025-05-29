using System;

namespace Domains.Scene.MainMenu
{
    public static class MainMenuEvents
    {
        public static event Action OnNewGameAttempted;
        public static event Action OnNewGameTriggered;
        public static event Action OnContinueGameTriggered;
        public static event Action OnQuitGameAttempted;
        public static event Action OnQuitGameTriggered;

        public static void TriggerNewGameAttempted() => OnNewGameAttempted?.Invoke();
        public static void TriggerNewGameTriggered() => OnNewGameTriggered?.Invoke();
        public static void TriggerContinueGameTriggered() => OnContinueGameTriggered?.Invoke();
        public static void TriggerQuitGameAttempted() => OnQuitGameAttempted?.Invoke();
        public static void TriggerQuitGameTriggered() => OnQuitGameTriggered?.Invoke();
    }
}