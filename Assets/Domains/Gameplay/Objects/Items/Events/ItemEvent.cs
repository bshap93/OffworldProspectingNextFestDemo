using Domains.Items.Inventory;
using MoreMountains.Tools;
using UnityEngine;

public enum ItemEventType
{
    Picked,
    Sold,
    Dropped
}

namespace Gameplay.Events
{
    public struct ItemEvent
    {
        private static ItemEvent _e;

        // Amount is embedded in the item
        public Inventory.InventoryEntry Item;
        public ItemEventType EventType;
        public Transform ItemTransform;

        public static void Trigger(ItemEventType eventType, Inventory.InventoryEntry inventoryItem, Transform transform)
        {
            _e.EventType = eventType;
            _e.Item = inventoryItem;
            _e.ItemTransform = transform;

            MMEventManager.TriggerEvent(_e);
        }
    }
}