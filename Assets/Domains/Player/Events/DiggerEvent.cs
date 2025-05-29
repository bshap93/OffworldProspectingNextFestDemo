using MoreMountains.Tools;

namespace Domains.Player.Events
{
    public enum DiggerEventType
    {
        Delete,
        Persist
    }

    public struct DiggerEvent
    {
        private static DiggerEvent _e;

        public DiggerEventType EventType;

        public static void Trigger(DiggerEventType eventType)
        {
            _e.EventType = eventType;
            MMEventManager.TriggerEvent(_e);
        }
    }
}