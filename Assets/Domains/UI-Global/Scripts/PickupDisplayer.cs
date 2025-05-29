using System.Collections;
using System.Collections.Generic;
using Domains.Items.Events;
using Domains.Items.Scripts;
using Domains.UI;
using Gameplay.Events;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using UnityEngine;

namespace Domains.UI_Global.Scripts
{
    /// <summary>
    ///     A class that displays notifications when items are picked up.
    ///     Should be attached to a GameObject with a layout group (Vertical or Horizontal).
    /// </summary>
    public class PickupDisplayer : MonoBehaviour, MMEventListener<ItemEvent>, MMEventListener<InventoryEvent>
    {
        [Tooltip("The prefab to use to display picked up items")]
        public PickupDisplayItem pickupDisplayPrefab;

        [Tooltip("The duration the pickup display item will remain on screen")]
        public float pickupDisplayDuration = 5f;

        [Tooltip("The fade in/out duration")] public float pickupFadeDuration = 0.2f;

        [SerializeField] private MMFeedbacks orePickupFeedbacks;
        [SerializeField] private MMFeedbacks stonePickupFeedbacks;
        [SerializeField] private MMFeedbacks bundlePickupFeedbacks;
        [SerializeField] private MMFeedbacks crystalPickupFeedbacks;

        // Dictionary to track active displays by item ID
        private readonly Dictionary<string, PickupDisplayItem> _displays = new();
        private WaitForSeconds _pickupDisplayWfs;

        private void OnEnable()
        {
            this.MMEventStartListening<ItemEvent>();
            this.MMEventStartListening<InventoryEvent>();
            OnValidate();
        }

        private void OnDisable()
        {
            this.MMEventStopListening<ItemEvent>();
            this.MMEventStopListening<InventoryEvent>();
        }

        private void OnValidate()
        {
            _pickupDisplayWfs = new WaitForSeconds(pickupDisplayDuration);
        }

        public void OnMMEvent(InventoryEvent eventType)
        {
            if (eventType.EventType == InventoryEventType.ContentChanged)
            {
                // Since we don't have a direct way to know it's a pickup specifically,
                // we need to track inventory additions some other way
                // This is handled through the ItemEvent in our case
            }
        }

        public void OnMMEvent(ItemEvent mmEvent)
        {
            if (mmEvent.EventType == ItemEventType.Picked)
            {
                DisplayPickedItem(mmEvent.Item.BaseItem, 1);
                
                switch (mmEvent.Item.BaseItem.ItemType)
                {
                    case ItemType.Bundle:
                        bundlePickupFeedbacks.PlayFeedbacks();
                        break;
                    case ItemType.Ore:
                        orePickupFeedbacks.PlayFeedbacks();
                        break;
                    case ItemType.Stone:
                        stonePickupFeedbacks.PlayFeedbacks();
                        break;
                    case ItemType.Crystal:
                        crystalPickupFeedbacks.PlayFeedbacks();
                        break;
                    default:
                        bundlePickupFeedbacks.PlayFeedbacks();
                        break;
                }
            }
        }

        public void DisplayPickedItem(BaseItem item, int quantity)
        {
            if (_displays.TryGetValue(item.ItemID, out var display))
            {
                // Item already being displayed, just update quantity
                display.AddQuantity(quantity);
            }
            else
            {
                // Create new display for this item
                _displays[item.ItemID] = Instantiate(pickupDisplayPrefab, transform);
                _displays[item.ItemID].Display(item, quantity);

                // Set up fade in
                var canvasGroup = _displays[item.ItemID].GetComponent<CanvasGroup>();
                if (canvasGroup)
                {
                    canvasGroup.alpha = 0;
                    StartCoroutine(MMFade.FadeCanvasGroup(canvasGroup, pickupFadeDuration, 1));
                }

                // Schedule fade out and destruction
                StartCoroutine(FadeOutAndDestroy(item.ItemID));
            }
        }

        private IEnumerator FadeOutAndDestroy(string itemID)
        {
            yield return _pickupDisplayWfs;

            if (_displays.TryGetValue(itemID, out var display))
            {
                var canvasGroup = display.GetComponent<CanvasGroup>();
                if (canvasGroup) yield return MMFade.FadeCanvasGroup(canvasGroup, pickupFadeDuration, 0);

                Destroy(display.gameObject);
                _displays.Remove(itemID);
            }
        }
    }
}