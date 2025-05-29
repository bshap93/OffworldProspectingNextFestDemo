using Domains.Player.Scripts;
using Domains.Scene.Events;
using Domains.UI_Global.Events;
using MoreMountains.Tools;
using UnityEngine;

namespace Domains.UI_Global.Scripts
{
    public class PauseOverlay : MonoBehaviour, MMEventListener<SceneEvent>
    {
        private CanvasGroup _canvasGroup;

        private void Start()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        private void OnEnable()
        {
            this.MMEventStartListening();
        }

        private void OnDisable()
        {
            this.MMEventStopListening();
        }

        public void OnMMEvent(SceneEvent eventType)
        {
            if (eventType.EventType == SceneEventType.TogglePauseScene)
            {
                var isPaused = _canvasGroup.alpha == 0;

                _canvasGroup.alpha = isPaused ? 1 : 0;
                _canvasGroup.interactable = isPaused;
                _canvasGroup.blocksRaycasts = isPaused;

                // ✅ Control the mouse cursor visibility and lock state
                Cursor.visible = isPaused;
                Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
            }
        }

        public void DieAndReset()
        {
            // Trigger the event to reset the player
            PlayerStatusEvent.Trigger(PlayerStatusEventType.ResetManaully);
            // Optionally, you can also trigger a UI event to close the pause menu
            UIEvent.Trigger(UIEventType.CloseUI);
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}