using System;
using MoreMountains.Tools;

namespace Domains.UI_Global.Events
{
    [Serializable]
    public enum TutorialEventType
    {
        PlayerUsedMoreInfo,
        PlayerSoldInitialItems
    }

    public struct TutorialEvent
    {
        public static TutorialEvent _e;

        public TutorialEventType EventType;

        public static void Trigger(TutorialEventType tutorialEventType)
        {
            _e.EventType = tutorialEventType;
            MMEventManager.TriggerEvent(_e);
        }
    }
}