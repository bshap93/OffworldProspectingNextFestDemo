using System;
using MoreMountains.Tools;

namespace Domains.Gameplay.Objectives.Events
{
    [Serializable]
    public enum ObjectiveEventType
    {
        ObjectiveCompleted,
        ObjectiveActivated,
        CompleteAllActiveObjectives,
        CompleteAllObjectivesPreviousTo
    }

    public enum NotifyType
    {
        Silent,
        Regular,
    }

    [Serializable]
    public struct ObjectiveEvent
    {
        private static ObjectiveEvent _e;

        public string objectiveId;
        public ObjectiveEventType type;
        
        public NotifyType notifyType;


        public static void Trigger(string objectiveId, ObjectiveEventType type, NotifyType notifyType = NotifyType.Regular)
        {
            _e.objectiveId = objectiveId;
            _e.notifyType = notifyType;
            _e.type = type;
            MMEventManager.TriggerEvent(_e);
        }
    }
}