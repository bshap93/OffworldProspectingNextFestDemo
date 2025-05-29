namespace Domains.Gameplay.Mining.Scripts
{
    public interface IInteractable
    {
        void Interact();

        void ShowInteractablePrompt();

        void HideInteractablePrompt();
    }
}