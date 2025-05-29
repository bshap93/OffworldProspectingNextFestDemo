using MoreMountains.Tools;

namespace Domains.UI_Global.Events
{
    public enum AudioEventType
    {
        ChangeVolume,
        Mute,
        Unmute
    }

    public struct AudioEvent
    {
        private static AudioEvent _e;
        public AudioEventType EventType;
        public float Value;


        public static void Trigger(AudioEventType eventType, float value)
        {
            _e.EventType = eventType;
            _e.Value = value;

            MMEventManager.TriggerEvent(_e);
        }
    }
}