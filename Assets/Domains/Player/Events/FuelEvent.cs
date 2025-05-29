using System;
using MoreMountains.Tools;

namespace Domains.Player.Events
{
    [Serializable]
    public enum FuelEventType
    {
        ConsumeFuel,
        RecoverFuel,
        FullyRecoverFuel,
        IncreaseMaximumFuel,

        SetMaxFuel,
        SetCurrentFuel,
        NotifyListeners,
        LowOnFuel
    }

    public struct FuelEvent
    {
        private static FuelEvent _e;

        public FuelEventType EventType;
        public float CurrentByValue;
        public float MaxByValue;

        public static void Trigger(FuelEventType fuelEventType,
            float byValue, float maxByValue)
        {
            _e.EventType = fuelEventType;

            _e.CurrentByValue = byValue;
            _e.MaxByValue = maxByValue;
            MMEventManager.TriggerEvent(_e);
        }
    }
}