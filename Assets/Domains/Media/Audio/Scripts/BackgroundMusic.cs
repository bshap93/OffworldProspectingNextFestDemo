using Domains.Gameplay.Managers.Scripts;
using Domains.UI_Global.Events;
using MoreMountains.Tools;
using UnityEngine;

namespace Domains.Media.Audio.Scripts
{ 
    public enum PlaybackMode
    {
        MainSequence,     // Normal looping through all clips
        CustomSequence,   // Looping through a subset of clips
        SingleClip,       // Play one clip then return to main sequence
        SingleClipLoop    // Loop a single clip indefinitely
    }

    public class BackgroundMusic : MonoBehaviour, MMEventListener<AudioEvent>
    {
        public AudioClip[] SoundClips;
        public bool Loop = true;
        public int ID = 255;
        [Range(0f, 1f)] public float volume = 0.8f;

        private int _currentClipIndex;
        private bool _isPlaying;
        private float _nextClipTime;

        private PlaybackMode _currentMode = PlaybackMode.MainSequence;
        private int[] _customSequence; // For custom playback sequences
        private int  _customSequenceIndex; // Current index in custom sequence
        private int _savedMainSequenceIndex;
        private bool _returnToMainAfterCurrent;
        private void Start()
        {
            this.MMEventStartListening();

            if (SoundClips != null && SoundClips.Length > 0) PlayNextClip();
        }

        private void Update()
        {
            if (!_isPlaying || PauseManager.Instance?.IsPaused() == true) return;

            if (Time.time >= _nextClipTime) HandleNextClip();
        }

        private void OnDisable()
        {
            this.MMEventStopListening();
        }

        public void OnMMEvent(AudioEvent eventType)
        {
            switch (eventType.EventType)
            {
                            case AudioEventType.ChangeVolume:
                                SetVolume(eventType.Value);
                                break;
                case AudioEventType.Mute:
                    SetVolume(0f);
                    break;
                case AudioEventType.Unmute:
                    SetVolume(volume);
                    break;
                case AudioEventType.PlayNextMusicClip:
                    PlayNextClip();
                    break;
                case AudioEventType.PlaySpecificMusicClip:
                    PlaySpecificClip(eventType.ClipIndex, eventType.Loop);
                    break;
            }


        }

        private void HandleNextClip()
        {
            switch (_currentMode)
            {
                case PlaybackMode.MainSequence:
                    PlayNextClipInMainSequence();
                    break;
                    
                case PlaybackMode.CustomSequence:
                    PlayNextClipInCustomSequence();
                    break;
                    
                case PlaybackMode.SingleClip:
                    ReturnToMainSequence();
                    break;
                    
                case PlaybackMode.SingleClipLoop:
                    PlayCurrentClip(true);
                    break;
            }
        }

        
        private void PlayNextClip()
        {
            if (_currentMode == PlaybackMode.MainSequence)
                PlayNextClipInMainSequence();
            else
                HandleNextClip();
        }
        
        private void PlayNextClipInMainSequence()
        {
            var clip = SoundClips[_currentClipIndex];
            _currentClipIndex = (_currentClipIndex + 1) % SoundClips.Length;
            PlayClip(clip, false);
        }
        
        private void PlayNextClipInCustomSequence()
        {
            if (_customSequence == null || _customSequence.Length == 0)
            {
                ReturnToMainSequence();
                return;
            }

            var clipIndex = _customSequence[_customSequenceIndex];
            _customSequenceIndex = (_customSequenceIndex + 1) % _customSequence.Length;
            
            if (clipIndex >= 0 && clipIndex < SoundClips.Length)
                PlayClip(SoundClips[clipIndex], false);
        }
        
        private void PlayCurrentClip(bool loop)
        {
            if (_currentClipIndex >= 0 && _currentClipIndex < SoundClips.Length)
                PlayClip(SoundClips[_currentClipIndex], loop);
        }
        
        private void PlayClip(AudioClip clip, bool loop)
        {
            var options = MMSoundManagerPlayOptions.Default;
            options.ID = ID;
            options.Loop = loop;
            options.Location = Vector3.zero;
            options.MmSoundManagerTrack = MMSoundManager.MMSoundManagerTracks.Music;
            options.Volume = volume;

            MMSoundManagerSoundPlayEvent.Trigger(clip, options);

            if (!loop)
                _nextClipTime = Time.time + clip.length;
            
            _isPlaying = true;
        }
        
        // Play a specific clip once, then return to main sequence
        public void PlaySpecificClip(int clipIndex, bool returnToMain = true)
        {
            if (clipIndex < 0 || clipIndex >= SoundClips.Length)
                return;

            if (returnToMain && _currentMode == PlaybackMode.MainSequence)
                _savedMainSequenceIndex = _currentClipIndex;

            _currentClipIndex = clipIndex;
            _currentMode = returnToMain ? PlaybackMode.SingleClip : PlaybackMode.SingleClipLoop;
            _returnToMainAfterCurrent = returnToMain;

            PlayClip(SoundClips[clipIndex], !returnToMain);
        }


        
        // Play a custom sequence of clips in a loop
        public void PlayCustomSequence(int[] clipIndices)
        {
            if (clipIndices == null || clipIndices.Length == 0)
                return;

            // Validate all indices
            foreach (int index in clipIndices)
            {
                if (index < 0 || index >= SoundClips.Length)
                {
                    UnityEngine.Debug.LogWarning($"Invalid clip index in custom sequence: {index}");
                    return;
                }
            }

            if (_currentMode == PlaybackMode.MainSequence)
                _savedMainSequenceIndex = _currentClipIndex;

            _customSequence = clipIndices;
            _customSequenceIndex = 0;
            _currentMode = PlaybackMode.CustomSequence;

            // Play first clip in custom sequence
            _currentClipIndex = _customSequence[0];
            _customSequenceIndex = 1;
            PlayClip(SoundClips[_currentClipIndex], false);
        }
        
        // Return to the main sequence from current position
        public void ReturnToMainSequence()
        {
            _currentMode = PlaybackMode.MainSequence;
            
            // Option 1: Continue from where we left off in main sequence
            _currentClipIndex = _savedMainSequenceIndex;
            
            // Option 2: Or start from beginning
            // _currentClipIndex = 0;
            
            PlayNextClipInMainSequence();
        }

        public void SetVolume(float newVolume)
        {
            volume = Mathf.Clamp01(newVolume);
            MMSoundManager.Instance.SetVolumeMusic(volume);
        }
        
        // Start looping a specific clip indefinitely
        public void LoopSpecificClip(int clipIndex)
        {
            PlaySpecificClip(clipIndex, false);
        }
        
        // Skip to next clip in current mode
        public void SkipToNext()
        {
            HandleNextClip();
        }
        
        // Check current playback mode
        public PlaybackMode GetCurrentMode()
        {
            return _currentMode;
        }



    }
}