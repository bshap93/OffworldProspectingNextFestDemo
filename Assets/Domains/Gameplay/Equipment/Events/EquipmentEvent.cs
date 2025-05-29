using System;
using Domains.Scripts_that_Need_Sorting;
using MoreMountains.Tools;

namespace Domains.Gameplay.Equipment.Events
{
    [Serializable]
    public enum EquipmentEventType
    {
        ChangeToEquipment,
        PickupEquipment
    }

    public struct EquipmentEvent
    {
        private static EquipmentEvent _e;

        public ToolType ToolType;
        public EquipmentEventType EventType;


        public static void Trigger(EquipmentEventType eventType, ToolType toolType
        )
        {
            _e.EventType = eventType;
            _e.ToolType = toolType;
            MMEventManager.TriggerEvent(_e);
        }
    }
}