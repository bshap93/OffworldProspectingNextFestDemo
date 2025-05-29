using System.Collections.Generic;
using Domains.Input.Scripts;
using Domains.Scene.Events;
using Domains.UI_Global.Events;
using Domains.UI_Global.Scripts;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Serialization;

namespace Domains.Gameplay.Managers.Scripts
{
    public class PauseManager : MonoBehaviour
    {
        public static PauseManager Instance;

        [SerializeField] private AudioSource uiButtonAudioSource;

        [SerializeField] private MMFeedbacks pauseFeedback;

        [FormerlySerializedAs("unPauseFeedback")] [SerializeField]
        private MMFeedbacks quitUIFeedbacks;

        private List<AudioSource> _audioSources = new();

        private bool _isPaused;

        private void Awake()
        {
            Instance = this;
        }


        private void Update()
        {
            if (CustomInputBindings.IsPausePressed() && !PlayerUIManager.Instance.uiIsOpen)
            {
                _isPaused = !_isPaused;
                Time.timeScale = _isPaused ? 0 : 1;
                if (_isPaused)
                {
                    _audioSources = new List<AudioSource>(FindObjectsByType<AudioSource>(FindObjectsSortMode.None));
                    foreach (var audioSource in _audioSources)
                        if (audioSource != null && audioSource != uiButtonAudioSource)
                            audioSource.Pause();
                }
                else
                {
                    foreach (var audioSource in _audioSources)
                        if (audioSource != null && audioSource != uiButtonAudioSource)
                            audioSource.UnPause();
                }


                SceneEvent.Trigger(SceneEventType.TogglePauseScene);
                pauseFeedback?.PlayFeedbacks();
            }
            else if (CustomInputBindings.IsPausePressed())
            {
                UIEvent.Trigger(UIEventType.CloseVendorConsole);
                UIEvent.Trigger(UIEventType.CloseFuelConsole);
                UIEvent.Trigger(UIEventType.CloseUI);
                UIEvent.Trigger(UIEventType.CloseBriefing);
                UIEvent.Trigger(UIEventType.CloseInfoPanel);
                UIEvent.Trigger(UIEventType.CloseInfoDump);
                UIEvent.Trigger(UIEventType.CloseCommsComputer);
                UIEvent.Trigger(UIEventType.CloseSettings);

                quitUIFeedbacks?.PlayFeedbacks();
            }
        }

        public bool IsPaused()
        {
            return _isPaused;
        }
    }
}