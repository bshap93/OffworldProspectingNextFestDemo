using MoreMountains.Tools;

namespace Domains.Effects.Highlighting
{
    public enum HighlightEventType
    {
        ActivateTarget,
        DeactivateTarget
    }

    public struct HighlightEvent
    {
        public static HighlightEvent _e;

        public string TargetID;


        public HighlightEventType EventType;

        public static void Trigger(HighlightEventType eventType, string targetId)
        {
            _e.EventType = eventType;
            _e.TargetID = targetId;
            MMEventManager.TriggerEvent(_e);
        }
    }
}