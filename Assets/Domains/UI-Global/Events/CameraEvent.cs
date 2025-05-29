using System;
using MoreMountains.Tools;

namespace Domains.UI_Global.Events
{
    [Serializable]
    public enum CameraEventType
    {
        None,
        CameraShake
    }

    public struct CameraEvent
    {
        public static CameraEvent _e;

        public CameraEventType EventType;

        public float Duration;

        public float Magnitude;

        public static void Trigger(CameraEventType eventType, float duration = 0f, float magnitude = 0f)
        {
            _e.EventType = eventType;
            _e.Duration = duration;
            _e.Magnitude = magnitude;
            MMEventManager.TriggerEvent(_e);
        }
    }
}