using System;
using MoreMountains.Tools;

namespace Domains.Debug.Events
{
    [Serializable]
    public enum DataSaveEventType
    {
        DataResetFinished
    }

    public struct DataSaveEvent
    {
        private static DataSaveEvent _e;
        public DataSaveEventType EventType;

        public static void Trigger(DataSaveEventType eventType)
        {
            _e.EventType = eventType;

            MMEventManager.TriggerEvent(_e);
        }
    }
}