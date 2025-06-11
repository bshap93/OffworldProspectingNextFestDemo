using System;
using System.Collections.Generic;
using System.Linq;
using Domains.Items.Events;
using Domains.Items.Scripts;
using Domains.Player.Events;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Serialization;

namespace Domains.Items.Inventory
{
    public class Inventory : MonoBehaviour
    {
        [FormerlySerializedAs("Content")] public List<InventoryEntry> content = new();

        [FormerlySerializedAs("InventoryFullFeedbacks")]
        public MMFeedbacks inventoryFullFeedbacks;

        public bool IsEmpty => content.Count == 0;

        // Get grouped items for UI display
        public List<GroupedItem> GetGroupedItems()
        {
            return content
                .GroupBy(item => item.BaseItem.ItemID)
                .Select(group => new GroupedItem(
                    group.First().BaseItem,
                    group.Count(),
                    group.Select(item => item.uniqueID).ToList()
                ))
                .ToList();
        }

        // Sell all items and add currency
        public void SellAllItems()
        {
            var totalValue = content.Sum(item => item.BaseItem.ItemValue);

            if (totalValue > 0) CurrencyEvent.Trigger(CurrencyEventType.AddCurrency, totalValue);

            content.Clear();
            InventoryEvent.Trigger(InventoryEventType.ContentChanged, this, 0);
        }

        [Serializable]
        public class GroupedItem
        {
            [FormerlySerializedAs("Item")] public BaseItem item;
            [FormerlySerializedAs("Quantity")] public int quantity;
            [FormerlySerializedAs("UniqueIDs")] public List<string> uniqueIDs;

            public GroupedItem(BaseItem item, int quantity, List<string> uniqueIDs)
            {
                this.item = item;
                this.quantity = quantity;
                this.uniqueIDs = uniqueIDs;
            }
        }

        [Serializable]
        public class InventoryEntry
        {
            [FormerlySerializedAs("UniqueID")] public string uniqueID;
            public BaseItem BaseItem;

            public InventoryEntry(string uniqueID, BaseItem item)
            {
                this.uniqueID = uniqueID;
                BaseItem = item;
            }
        }
    }
}