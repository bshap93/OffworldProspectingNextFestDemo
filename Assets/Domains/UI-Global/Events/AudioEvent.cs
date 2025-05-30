using MoreMountains.Tools;

namespace Domains.UI_Global.Events
{
    public enum AudioEventType
    {
        ChangeVolume,
        Mute,
        Unmute, 
        PlayNextMusicClip,
        PlaySpecificMusicClip,
    }

    public struct AudioEvent
    {
        private static AudioEvent _e;
        public AudioEventType EventType;
        public float Value;
        public int ClipIndex; // For specific clip playback
        public bool Loop;


        public static void Trigger(AudioEventType eventType, float value, int clipIndex = -1, bool loop = true)
        {
            _e.EventType = eventType;
            _e.Value = value;
            _e.ClipIndex = clipIndex;
            _e.Loop = loop;

            MMEventManager.TriggerEvent(_e);
        }
    }
}