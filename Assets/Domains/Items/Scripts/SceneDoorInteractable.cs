using System.Collections;
using Domains.UI_Global.Events;
using UnityEngine;

namespace Domains.Items.Scripts
{
    public class SceneDoorInteractable : InteractableObjective

    {
        private ConditionalDoor _conditionalDoor;

        protected override void Start()
        {
            base.Start();
            StartCoroutine(InitializeAfterProgressionManager());

            _conditionalDoor = GetComponent<ConditionalDoor>();
        }

        public override void Interact()
        {
            if (_conditionalDoor != null)
                if (_conditionalDoor.GetLockedState())
                {
                    AlertEvent.Trigger(AlertReason.GatewayClosed, "The gateway is locked.", "Gateway Locked", null,
                        null, Color.red);
                }
                else

                {
                    interactFeedbacks?.PlayFeedbacks();
                    OnInteractableInteract?.Invoke();
                }
        }

        protected override IEnumerator InitializeAfterProgressionManager()
        {
            yield return null;
        }
    }
}