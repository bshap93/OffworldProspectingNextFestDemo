using MoreMountains.Tools;

namespace Domains.Items.Events
{
    public enum InventoryEventType
    {
        ContentChanged,
        InventoryLoaded,
        SellAllItems,
        UpgradedWeightLimit
    }

    public struct InventoryEvent
    {
        public static InventoryEvent E;

        public InventoryEventType EventType;

        public Inventory.Inventory Inventory;

        public float CurrentWeight;

        public float WeightLimitIncrease;

        public static void Trigger(InventoryEventType eventType, Inventory.Inventory inventory, float weightLimit)
        {
            E.EventType = eventType;
            E.Inventory = inventory;


            E.WeightLimitIncrease = weightLimit;

            MMEventManager.TriggerEvent(E);
        }
    }
}