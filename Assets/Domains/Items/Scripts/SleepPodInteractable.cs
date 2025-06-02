using System.Collections;
using Domains.Input.Scripts;

namespace Domains.Items.Scripts
{
    public class SleepPodInteractable : InteractableObjective
    {
        private InfoPanelActivator _infoPanelActivator;

        protected override void Start()
        {
            base.Start();
            _infoPanelActivator = GetComponent<InfoPanelActivator>();
            if (interactFeedbacks != null) interactFeedbacks.Initialization();
        }

        public override void Interact()
        {
            if (hasBeenInteractedWith) return;
            if (_infoPanelActivator != null) _infoPanelActivator.ToggleInfoPanel();
        }

        protected override IEnumerator InitializeAfterProgressionManager()
        {
            yield return null;
        }
    }
}