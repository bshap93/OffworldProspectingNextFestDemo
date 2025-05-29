using Domains.Gameplay.Mining.Scripts;
using Domains.Player.Events;
using Domains.SaveLoad;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Domains.Input.Scripts
{
    public class ButtonActivatedWithAction : MonoBehaviour, IInteractable
    {
        public UnityEvent OnActivation;
        public ButtonPromptWithAction ButtonPromptPrefab;

        public float scaleFactor = 1f;

        [Header("Prompt Settings")] public bool isShowing;

        public Vector3 promptTransformOffset;
        public Vector3 promptRotationOffset;

        public MMFeedbacks activationFeedback;

        [FormerlySerializedAs("PromptActionText")]
        public string PromptActionStr = "Interact";

        public Color PromptTextColor = Color.white;

        [FormerlySerializedAs("PromptKeyText")]
        public string PromptKeyStr = "E";

        private ButtonPromptWithAction _buttonPrompt;

        private InfoPanelActivator _infoPanelActivator;


        private void Start()
        {
            if (ButtonPromptPrefab != null)
            {
                var promptPosition = transform.position + promptTransformOffset;
                var promptRotation = Quaternion.Euler(promptRotationOffset);
                _buttonPrompt = Instantiate(ButtonPromptPrefab, promptPosition, promptRotation, transform);
                _buttonPrompt.transform.localScale *= scaleFactor;
                _buttonPrompt.SetTextColor(PromptTextColor);
                _buttonPrompt.Initialization();
                _buttonPrompt.Hide();
            }

            _infoPanelActivator = GetComponent<InfoPanelActivator>();
        }

        private void Update()
        {
            if (isShowing)
                if (CustomInputBindings.IsInteractPressed())
                    Interact();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player")) ShowInteractablePrompt();
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player")) HideInteractablePrompt();
        }


        public void Interact()
        {
            if (_infoPanelActivator != null)
                if (_infoPanelActivator.automaticallyShowOnInteract)
                    _infoPanelActivator.ShowInfoPanel();
                else
                    _infoPanelActivator.HideInfoPanel();
            ActivateButton();
        }

        public void ShowInteractablePrompt()
        {
            if (_buttonPrompt != null)
            {
                isShowing = true;
                _buttonPrompt.Show(PromptKeyStr, PromptActionStr);
            }
        }

        public void HideInteractablePrompt()
        {
            if (_buttonPrompt != null)
            {
                isShowing = false;
                _buttonPrompt.Hide();
            }
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

        public void HidePrompt()
        {
            if (_buttonPrompt != null) _buttonPrompt.Hide();
        }
    }
}