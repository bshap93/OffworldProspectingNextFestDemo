using Domains.Gameplay.Managers.Scripts;
using Domains.UI_Global.Events;
using MoreMountains.Tools;
using UnityEngine;

namespace Domains.Audio.Scripts
{
    public class BackgroundMusic : MonoBehaviour, MMEventListener<AudioEvent>
    {
        public AudioClip[] SoundClips;
        public bool Loop = true;
        public int ID = 255;
        [Range(0f, 1f)] public float volume = 1f;

        private int _currentClipIndex;
        private bool _isPlaying;
        private float _nextClipTime;

        private void Start()
        {
            this.MMEventStartListening();

            if (SoundClips != null && SoundClips.Length > 0) PlayNextClip();
        }

        private void Update()
        {
            if (!_isPlaying || PauseManager.Instance?.IsPaused() == true) return;

            if (Time.time >= _nextClipTime) PlayNextClip();
        }

        private void OnDisable()
        {
            this.MMEventStopListening();
        }

        public void OnMMEvent(AudioEvent eventType)
        {
            if (eventType.EventType == AudioEventType.ChangeVolume)
                SetVolume(eventType.Value);
            else if (eventType.EventType == AudioEventType.Mute)
                SetVolume(0f);
            else if (eventType.EventType == AudioEventType.Unmute)
                SetVolume(volume);
        }

        private void PlayNextClip()
        {
            var clip = SoundClips[_currentClipIndex];
            _currentClipIndex = (_currentClipIndex + 1) % SoundClips.Length;

            var options = MMSoundManagerPlayOptions.Default;
            options.ID = ID;
            options.Loop = false;
            options.Location = Vector3.zero;
            options.MmSoundManagerTrack = MMSoundManager.MMSoundManagerTracks.Music;
            options.Volume = volume;

            MMSoundManagerSoundPlayEvent.Trigger(clip, options);

            _nextClipTime = Time.time + clip.length;
            _isPlaying = true;
        }

        public void SetVolume(float newVolume)
        {
            volume = Mathf.Clamp01(newVolume);
            MMSoundManager.Instance.SetVolumeMusic(volume);
        }
    }
}