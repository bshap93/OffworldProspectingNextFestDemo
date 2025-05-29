using Domains.Scripts_that_Need_Sorting;
using MoreMountains.Tools;

namespace Domains.Gameplay.Mining.Events
{
    public enum ToolEventType
    {
        UseTool,
        UpgradeTool
    }

    public struct ToolEvent
    {
        public static ToolEvent _e;

        public ToolEventType EventType;
        public ToolType ToolType;
        public ToolIteration ToolIteration;


        public static void Trigger(ToolEventType eventType, ToolType toolType, ToolIteration toolIteration
        )
        {
            _e.EventType = eventType;
            _e.ToolType = toolType;
            _e.ToolIteration = toolIteration;
            MMEventManager.TriggerEvent(_e);
        }
    }
}