using Domains.Gameplay.Mining.Scripts;
using Domains.Player.Events;
using Domains.SaveLoad;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Domains.Input.Scripts
{
    public class ButtonActivated : MonoBehaviour, IInteractable
    {
        public UnityEvent OnActivation;
        public ButtonPrompt ButtonPromptPrefab;

        public Vector3 promptTransformOffset;
        public Vector3 promptRotationOffset;

        public MMFeedbacks activationFeedback;

        public float scaleFactor = 1f;

        public Color PromptTextColor = Color.white;

        public float ShowPromptDelay;

        [FormerlySerializedAs("PromptKeyText")]
        public string PromptKeyStr = "E";

        private ButtonPrompt _buttonPrompt;

        private void Start()
        {
            if (ButtonPromptPrefab != null)
            {
                var promptPosition = transform.position + promptTransformOffset;
                var promptRotation = Quaternion.Euler(promptRotationOffset);
                _buttonPrompt = Instantiate(ButtonPromptPrefab, promptPosition, promptRotation, transform);
                _buttonPrompt.transform.localScale *= scaleFactor;
                _buttonPrompt.Initialization();
                _buttonPrompt.Hide();
            }
        }

        public void Interact()
        {
            ActivateButton();
        }

        public void ShowInteractablePrompt()
        {
            if (_buttonPrompt != null) _buttonPrompt.Show(PromptKeyStr);
        }

        public void HideInteractablePrompt()
        {
            if (_buttonPrompt != null) _buttonPrompt.Hide();
        }

        private void ActivateButton()
        {
            if (OnActivation != null)
            {
                OnActivation.Invoke();
                activationFeedback?.PlayFeedbacks();
            }
        }


        public void TriggerSave()
        {
            SaveLoadEvent.Trigger(SaveLoadEventType.Save);
        }

        public void TriggerHealthRestore()
        {
            HealthEvent.Trigger(HealthEventType.RecoverHealth, 100);
        }
    }
}