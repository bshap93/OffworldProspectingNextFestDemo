using Domains.Input.Scripts;
using Domains.Items.Events;
using Domains.Items.Inventory;
using Domains.UI_Global.Events;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Serialization;

namespace Domains.Player.Interaction
{
    public class TrainConsole : MonoBehaviour
    {
        [FormerlySerializedAs("SellAllFeedbacks")]
        public MMFeedbacks sellAllFeedbacks;

        [SerializeField] private InfoPanelActivator infoPanelActivator;
        public bool hasBeenIntroduced;


        private Inventory _inventory;


        private void Start()
        {
            _inventory = FindFirstObjectByType<Inventory>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!hasBeenIntroduced)
            {
                hasBeenIntroduced = true;
                infoPanelActivator?.ShowInfoPanel();
            }
        }


        private void OnTriggerExit(Collider other)
        {
            if (hasBeenIntroduced) infoPanelActivator?.HideInfoPanel();
        }

        public void TriggerSellAll()
        {
            sellAllFeedbacks?.PlayFeedbacks();
            InventoryEvent.Trigger(InventoryEventType.SellAllItems, _inventory, 0);
        }

        public void TriggerOpenVendorUI()
        {
            UIEvent.Trigger(UIEventType.OpenVendorConsole);
        }
    }
}