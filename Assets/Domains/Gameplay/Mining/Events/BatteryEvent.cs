using System;
using MoreMountains.Tools;

namespace Domains.Mining.Events
{
    [Serializable]
    public enum BatteryEventType
    {
        Charge,
        Discharge
    }
    public struct BatteryEvent
    {
        static BatteryEvent _e;
        
        public BatteryEventType EventType;
        public int ChargeAmount;
        
        public static void Trigger(BatteryEventType eventType, int chargeAmount)
        {
            _e.EventType = eventType;
            _e.ChargeAmount = chargeAmount;
            MMEventManager.TriggerEvent(_e);
        }
    }
}
