using MoreMountains.Tools;

namespace Domains.Player.Events
{
    public enum ProgressionEventType
    {
        FinishTutorial,
        StartTutorial,
        CollectedObjective
    }

    public struct ProgressionEvent
    {
        private static ProgressionEvent _e;

        public ProgressionEventType EventType;
        public string UniqueID;

        public static void Trigger(ProgressionEventType eventType, string uniqueID = null)
        {
            _e.EventType = eventType;
            _e.UniqueID = uniqueID;

            MMEventManager.TriggerEvent(_e);
        }
    }
}