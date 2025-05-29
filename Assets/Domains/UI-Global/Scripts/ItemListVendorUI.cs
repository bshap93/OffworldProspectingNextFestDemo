using System.Collections.Generic;
using Domains.Items.Events;
using Domains.Items.Inventory;
using MoreMountains.Tools;
using UnityEngine;

public class ItemListVendorUI : MonoBehaviour, MMEventListener<InventoryEvent>
{
    public GameObject list;
    public GameObject itemElementPrefab;
    public List<GameObject> itemElements = new();
    public Inventory mainInventory;

    private void Start()
    {
        RefreshItemList();
    }

    private void OnEnable()
    {
        this.MMEventStartListening();
    }

    private void OnDisable()
    {
        this.MMEventStopListening();
    }

    public void OnMMEvent(InventoryEvent eventType)
    {
        if (eventType.EventType == InventoryEventType.ContentChanged) RefreshItemList();
    }

    public void RefreshItemList()
    {
        // Clear existing item elements
        foreach (var element in itemElements) Destroy(element);
        itemElements.Clear();

        // Get grouped items directly from the inventory
        var groupedItems = mainInventory.GetGroupedItems();

        // Create UI elements for each group
        foreach (var group in groupedItems)
        {
            var itemElement = Instantiate(itemElementPrefab, list.transform);
            var elementComponent = itemElement.GetComponent<ItemElementVendor>();

            // Set the image
            elementComponent.ItemImage.sprite = group.item.ItemIcon;

            // Set the quantity text
            elementComponent.ItemQuantity.text = group.quantity.ToString();
            elementComponent.ItemName.text = group.item.ItemName;
            elementComponent.ItemValue.text = group.item.ItemValue.ToString();


            // You can also attach the item data for use in click handlers, etc.
            var elementData = itemElement.AddComponent<ItemElementData>();
            elementData.ItemID = group.item.ItemID;
            elementData.UniqueIDs = group.uniqueIDs;

            itemElements.Add(itemElement);
        }
    }
}