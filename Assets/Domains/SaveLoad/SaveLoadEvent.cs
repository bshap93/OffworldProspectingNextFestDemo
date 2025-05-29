using MoreMountains.Tools;

namespace Domains.SaveLoad
{
    public enum SaveLoadEventType
    {
        Save,
        Load
    }

    public struct SaveLoadEvent
    {
        public static SaveLoadEvent E;

        public SaveLoadEventType EventType;

        public static void Trigger(SaveLoadEventType eventType)
        {
            E.EventType = eventType;
            MMEventManager.TriggerEvent(E);
        }
    }
}