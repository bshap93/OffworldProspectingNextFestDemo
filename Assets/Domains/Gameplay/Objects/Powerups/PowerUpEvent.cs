using System;
using MoreMountains.Tools;

namespace Domains.Gameplay.Objects.Powerups
{
    [Serializable]
    public enum PowerUpEventType
    {
        PowerUpActivated,
        PowerUpExpired
    }

    [Serializable]
    public struct PowerUpEvent
    {
        private static PowerUpEvent _e;

        public PowerUpEventType PowerUpEventType;

        public PowerUpScriptableObject PowerUpScriptableObject;

        public static void Trigger(PowerUpEventType powerUpEventType, PowerUpScriptableObject powerUp)
        {
            _e.PowerUpEventType = powerUpEventType;
            _e.PowerUpScriptableObject = powerUp;
            MMEventManager.TriggerEvent(_e);
        }
    }
}