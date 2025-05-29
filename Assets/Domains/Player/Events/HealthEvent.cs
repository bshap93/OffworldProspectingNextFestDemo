using System;
using MoreMountains.Tools;

namespace Domains.Player.Events
{
    [Serializable]
    public enum HealthEventType
    {
        ConsumeHealth,
        RecoverHealth,
        FullyRecoverHealth,
        IncreaseMaximumHealth,
        DecreaseMaximumHealth,
        SetCurrentHealth
    }

    [Serializable]
    public enum HealthEventReason
    {
        FallDamage,
        LavaDamage
    }

    public struct HealthEvent
    {
        private static HealthEvent e;

        public HealthEventType EventType;
        public float ByValue;

        public HealthEventReason? Reason;

        public static void Trigger(HealthEventType healthEventType,
            float byValue, HealthEventReason? reason = null)
        {
            e.EventType = healthEventType;
            e.ByValue = byValue;
            e.Reason = reason;
            MMEventManager.TriggerEvent(e);
        }
    }
}