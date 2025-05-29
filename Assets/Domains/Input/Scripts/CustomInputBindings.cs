using UnityEngine;

namespace Domains.Input.Scripts
{
    public class CustomInputBindings
    {
        // Define keybindings in one place
        private const KeyCode InteractKey = KeyCode.E;
        private const KeyCode CrouchKey = KeyCode.LeftControl;
        private const KeyCode RunKey = KeyCode.LeftShift;
        private const KeyCode ChangePerspectiveKey = KeyCode.V;
        private const KeyCode PersistanceKey = KeyCode.P;
        private const KeyCode DeletionKey = KeyCode.Alpha0;
        private const KeyCode SaveDebugKey = KeyCode.F5;
        private const KeyCode PauseKey = KeyCode.Escape;
        private const KeyCode QuestJournalKey = KeyCode.J;
        private const int MineMouseButton = 0;
        private const int GetMoreInfoPressed = 1;


        // Methods to check input (abstraction layer)
        public static bool IsInteractPressed()
        {
            return UnityEngine.Input.GetKeyDown(InteractKey);
        }

        public static bool IsGetMoreInfoPressed()
        {
            return UnityEngine.Input.GetMouseButton(GetMoreInfoPressed);
        }

        public static bool IsPausePressed()
        {
            return UnityEngine.Input.GetKeyDown(PauseKey);
        }

        public static bool IsSaveDebugKeyPressed()
        {
            return UnityEngine.Input.GetKeyDown(SaveDebugKey);
        }

        public static bool IsResetHeld()
        {
            return UnityEngine.Input.GetKey(InteractKey);
        }


        public static bool IsPersistanceKeyPressed()
        {
            return UnityEngine.Input.GetKeyDown(PersistanceKey);
        }


        public static bool IsMineMouseButtonPressed()
        {
            return UnityEngine.Input.GetMouseButton(MineMouseButton);
        }

        public static bool IsChangingWeapons()
        {
            return UnityEngine.Input.mouseScrollDelta.y != 0;
        }

        public static int GetWeaponChangeDirection()
        {
            return UnityEngine.Input.mouseScrollDelta.y > 0 ? 1 : -1;
        }
    }
}