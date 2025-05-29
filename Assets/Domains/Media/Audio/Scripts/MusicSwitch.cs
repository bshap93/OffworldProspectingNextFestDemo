using MoreMountains.Tools;
using UnityEngine;

namespace Domains.Audio
{
    [AddComponentMenu("TopDown Engine/GUI/Music Switch")]
    public class MusicSwitch : MonoBehaviour
    {
        public virtual void On()
        {
            MMSoundManagerTrackEvent.Trigger(MMSoundManagerTrackEventTypes.UnmuteTrack,
                MMSoundManager.MMSoundManagerTracks.Music);
        }

        public virtual void Off()
        {
            MMSoundManagerTrackEvent.Trigger(MMSoundManagerTrackEventTypes.MuteTrack,
                MMSoundManager.MMSoundManagerTracks.Music);
        }
    }
}