using MoreMountains.Tools;

namespace Domains.Player.Events
{
    public enum DestructableEventType
    {
        Destroyed
    }

    public struct DestructableEvent
    {
        private static DestructableEvent _e;
        public string UniqueID;

        public DestructableEventType EventType;

        public static void Trigger(DestructableEventType eventType, string uniqueID)
        {
            _e.EventType = eventType;
            _e.UniqueID = uniqueID;
            MMEventManager.TriggerEvent(_e);
        }
    }
}