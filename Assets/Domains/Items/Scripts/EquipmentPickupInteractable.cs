using System.Collections;
using Domains.Gameplay.Equipment.Events;
using Domains.Gameplay.Objectives.Events;
using Domains.Input.Scripts;
using Domains.Player.Events;
using Domains.Player.Progression;
using Domains.Scripts_that_Need_Sorting;
using Domains.UI_Global.Events;
using UnityEngine;

namespace Domains.Items.Scripts
{
    public class EquipmentPickupInteractable : InteractableObjective
    {
        [SerializeField] private ToolType toolType;
        private InfoPanelActivator _infoPanelActivator;
        

        protected override void Start()
        {
            base.Start();
            StartCoroutine(InitializeAfterProgressionManager());

            _infoPanelActivator = GetComponent<InfoPanelActivator>();
            if (interactFeedbacks != null) interactFeedbacks.Initialization();
        }

        protected override IEnumerator InitializeAfterProgressionManager()
        {
            yield return null;


            if (ProgressionManager.IsObjectiveCollected(uniqueID)) Destroy(gameObject);
        }

        public override void Interact()
        {
            if (hasBeenInteractedWith) return;
            CurrencyEvent.Trigger(CurrencyEventType.AddCurrency, rewardAmount);
            if (_infoPanelActivator != null) _infoPanelActivator.ToggleInfoPanel();
        }

        public void PickupEquipment()
        {
            if (toolType == ToolType.Jetpack)
            {
                EquipmentEvent.Trigger(EquipmentEventType.PickupEquipment, ToolType.Jetpack);
                AlertEvent.Trigger(AlertReason.PickedUpEquipment,
                    "Picked up Jetpack", "Jetpack Picked Up", null, null, Color.white);
            }

            if (toolType == ToolType.DemoGift)
            {
                EquipmentEvent.Trigger(EquipmentEventType.PickupEquipment, ToolType.DemoGift);
                AlertEvent.Trigger(AlertReason.PickedUpEquipment,
                    "Picked up Demo Gift", "Demo Gift Picked Up", null, null, Color.white);
            }


            interactFeedbacks?.PlayFeedbacks();
            hasBeenInteractedWith = true;
            ProgressionManager.AddInteractableObjective(uniqueID, true);
            OnInteractableInteract?.Invoke();
            Destroy(gameObject);
        }
    }
}