using System;
using MoreMountains.Tools;

namespace Domains.Gameplay.Objectives.Events
{
    [Serializable]
    public enum ObjectiveEventType
    {
        ObjectiveCompleted,
        ObjectiveActivated,
        CompleteAllActiveObjectives
    }

    [Serializable]
    public struct ObjectiveEvent
    {
        private static ObjectiveEvent _e;

        public string objectiveId;
        public ObjectiveEventType type;


        public static void Trigger(string objectiveId, ObjectiveEventType type)
        {
            _e.objectiveId = objectiveId;
            _e.type = type;
            MMEventManager.TriggerEvent(_e);
        }
    }
}