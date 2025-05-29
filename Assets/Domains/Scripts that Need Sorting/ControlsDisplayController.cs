using Domains.Input.Events;
using Domains.Player.Progression;
using MoreMountains.Tools;
using UnityEngine;

namespace Domains.Scripts_that_Need_Sorting
{
    public class ControlsDisplayController : MonoBehaviour, MMEventListener<InputSettingsEvent>
    {
        private CanvasGroup canvasGroup;


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                UnityEngine.Debug.LogError("ControlsDisplayController: No CanvasGroup found on this GameObject.");

            var showKeyboardControls = PlayerPrefs.GetInt("ShowKeyboardControls", 1) == 1;
            if (showKeyboardControls)
                ShowControls();
            else
                HideControls();
        }

        // Update is called once per frame
        private void Update()
        {
            // if (CustomInputBindings.IsGetMoreInfoPressed())
            //     ShowControls();
            if (ProgressionManager.TutorialFinished)
                HideControls();
        }

        private void OnEnable()
        {
            this.MMEventStartListening();
        }

        private void OnDisable()
        {
            this.MMEventStopListening();
        }

        public void OnMMEvent(InputSettingsEvent eventType)
        {
            if (eventType.EventType == InputSettingsEventType.ShowKeyboardControls && eventType.BoolValue == true)
                ShowControls();
            else if (eventType.EventType == InputSettingsEventType.ShowKeyboardControls) HideControls();
        }

        private void ShowControls()
        {
            if (canvasGroup != null) canvasGroup.alpha = 1f;
        }

        private void HideControls()
        {
            if (canvasGroup != null) canvasGroup.alpha = 0f;
        }
    }
}