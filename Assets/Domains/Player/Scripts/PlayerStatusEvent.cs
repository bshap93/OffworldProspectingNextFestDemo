using System;
using Domains.Player.Events;
using MoreMountains.Tools;

namespace Domains.Player.Scripts
{
    [Serializable]
    public enum PlayerStatusEventType
    {
        OutOfFuel,
        Died,
        RegainedHealth,
        ImmuneToDamage,
        ResetHealth,
        ResetFuel,
        RegainedFuel,
        SoftReset,
        ResetManaully
    }

    [Serializable]
    public enum PlayerStatusEventReason
    {
        FallDamage,
        LavaDamage
    }

    public struct PlayerStatusEvent
    {
        private static PlayerStatusEvent e;

        public PlayerStatusEventType EventType;


        public HealthEventReason? Reason;

        public static void Trigger(PlayerStatusEventType eventType, HealthEventReason? reason = null)
        {
            e.EventType = eventType;
            e.Reason = reason;
            MMEventManager.TriggerEvent(e);
        }
    }
}