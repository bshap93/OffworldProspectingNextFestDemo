using Domains.Player.Events;
using Domains.Player.Scripts;
using MoreMountains.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Domains.UI_Global.UIUpdaters
{
    public class HealthBarUpdater : MonoBehaviour, MMEventListener<HealthEvent>, MMEventListener<PlayerStatusEvent>
    {
        [SerializeField] private Slider healthBarSlider;
        [SerializeField] private TMP_Text healthPercentageText;

        private float _currentHealth;
        private float _maxHealth;

        private void Start()
        {
            Initialize();
        }

        // Update every frame to catch any changes from different sources
        private void Update()
        {
            // Check if values are out of sync and update if needed
            if (_currentHealth != PlayerHealthManager.HealthPoints ||
                _maxHealth != PlayerHealthManager.MaxHealthPoints)
            {
                _currentHealth = PlayerHealthManager.HealthPoints;
                _maxHealth = PlayerHealthManager.MaxHealthPoints;
                UpdateBar();
            }
        }

        private void OnEnable()
        {
            this.MMEventStartListening<HealthEvent>();
            this.MMEventStartListening<PlayerStatusEvent>();
        }

        private void OnDisable()
        {
            this.MMEventStopListening<HealthEvent>();
            this.MMEventStopListening<PlayerStatusEvent>();
        }

        public void OnMMEvent(HealthEvent eventType)
        {
            switch (eventType.EventType)
            {
                case HealthEventType.ConsumeHealth:
                    _currentHealth = PlayerHealthManager.HealthPoints; // Get directly from manager
                    break;
                case HealthEventType.RecoverHealth:
                    _currentHealth = PlayerHealthManager.HealthPoints; // Get directly from manager
                    break;
                case HealthEventType.FullyRecoverHealth:
                    _currentHealth = PlayerHealthManager.MaxHealthPoints;
                    break;
                case HealthEventType.IncreaseMaximumHealth:
                    _maxHealth = PlayerHealthManager.MaxHealthPoints;
                    break;
                case HealthEventType.DecreaseMaximumHealth:
                    _maxHealth = PlayerHealthManager.MaxHealthPoints;
                    break;
                case HealthEventType.SetCurrentHealth:
                    _currentHealth = PlayerHealthManager.HealthPoints;
                    break;
            }

            UpdateBar();
        }

        public void OnMMEvent(PlayerStatusEvent eventType)
        {
            // Refresh the health bar on player status events
            if (eventType.EventType == PlayerStatusEventType.RegainedHealth ||
                eventType.EventType == PlayerStatusEventType.ResetHealth ||
                eventType.EventType == PlayerStatusEventType.Died)
            {
                _currentHealth = PlayerHealthManager.HealthPoints;
                _maxHealth = PlayerHealthManager.MaxHealthPoints;
                UpdateBar();
            }
        }

        public void Initialize()
        {
            _maxHealth = PlayerHealthManager.MaxHealthPoints;
            _currentHealth = PlayerHealthManager.HealthPoints;
            UpdateBar();
        }

        private void UpdateBar()
        {
            if (_maxHealth <= 0) _maxHealth = 1; // Prevent division by zero

            healthBarSlider.value = Mathf.Clamp01(_currentHealth / _maxHealth);
            healthPercentageText.text = $"{healthBarSlider.value * 100:0}%";
        }
    }
}