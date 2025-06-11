using Domains.Gameplay.Mining.Scripts;
using Domains.Input.Scripts;
using UnityEngine;
using UnityEngine.Events;

namespace Domains.Gameplay.Objects.Powerups
{
    public class PowerUpInteractable : MonoBehaviour, IInteractable
    {
        public string uniqueId;
        public PowerUpScriptableObject powerUpScriptableObject;
        public UnityEvent onInteract;

        public void Interact()
        {
            // If interaction key is still held
            if (CustomInputBindings.IsInteractPressed()) UsePowerUp();
        }

        public void ShowInteractablePrompt()
        {
        }

        public void HideInteractablePrompt()
        {
        }

        public void UsePowerUp()
        {
            PowerUpEvent.Trigger(PowerUpEventType.PowerUpActivated, powerUpScriptableObject);
            onInteract?.Invoke();
            Destroy(gameObject);
        }
    }
}