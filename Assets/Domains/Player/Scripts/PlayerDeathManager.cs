using System.Collections;
using Domains.Player.Events;
using Domains.Scene.Scripts;
using Domains.UI_Global.Events;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Serialization;

namespace Domains.Player.Scripts
{
    public class PlayerDeathManager : MonoBehaviour, MMEventListener<PlayerStatusEvent>

    {
        private static PlayerDeathManager _instance;

        [FormerlySerializedAs("teleportWhenOutOfFuel")] [Header("Game Mechanics")]
        public bool autoResetWhenOutOfFuel = true; // Toggle in inspector

        public float monetaryPenalty = 400f;

        [FormerlySerializedAs("staminaPenaltyMultiplier")]
        public float fuelPenaltyMultiplier = 0.2f;

        public float healthPenaltyMultiplier = 0.2f;


        [Header("Death Feedbacks")] public MMFeedbacks fallDeathFeedbacks;
        public MMFeedbacks lavaDeathFeedbacks;
        public MMFeedbacks manualResetDeathFeedbacks;

        public MMFeedbacks outOfFuelFeedbacks;

        [Header("Revival Feedbacks")] public MMFeedbacks revivalFeedbacks;

        [FormerlySerializedAs("_sceneRestarter")] [SerializeField]
        private MMSceneRestarter sceneRestarter;

        [SerializeField] private Transform spawnPoint;
        [SerializeField] private GameObject playerCamera;


        private void Awake()
        {
            _instance = this;
        }


        private void OnEnable()
        {
            this.MMEventStartListening();
        }

        private void OnDisable()
        {
            this.MMEventStopListening();
        }

        public void OnMMEvent(PlayerStatusEvent eventType)
        {
            if (eventType.EventType == PlayerStatusEventType.Died)
            {
                SetPostDeathStats();
                if (eventType.Reason != null)
                {
                    switch (eventType.Reason)
                    {
                        case HealthEventReason.FallDamage:
                            fallDeathFeedbacks?.PlayFeedbacks();
                            AlertEvent.Trigger(AlertReason.Died,
                                "You fell and passed out! You're charged 400 credits for your rescue.",
                                "Fall Emergency");
                            break;
                        case HealthEventReason.LavaDamage:
                            lavaDeathFeedbacks?.PlayFeedbacks();
                            AlertEvent.Trigger(AlertReason.Died,
                                "You walked into lava! You're charged 400 credits for your rescue.", "Lava Emergency");
                            break;
                    }
                }
                else
                {
                    fallDeathFeedbacks?.PlayFeedbacks();
                    AlertEvent.Trigger(AlertReason.Died, "You passed out!",
                        "You're charged 400 credits for your rescue.");
                }

                StartCoroutine(TriggerRevivalFeedbacks());
            }


            if (eventType.EventType == PlayerStatusEventType.OutOfFuel)

            {
                outOfFuelFeedbacks?.StopFeedbacks();
                AlertEvent.Trigger(AlertReason.OutOfFuel, "You are out of fuel! Hold E to Reset", "Out of Fuel");
            }

            if (eventType.EventType == PlayerStatusEventType.SoftReset)
            {
                SetPostFuelOutStats();
                outOfFuelFeedbacks?.PlayFeedbacks();
                AlertEvent.Trigger(AlertReason.OutOfFuel, "You are out of fuel!", "Out of Fuel");
            }
        }

        private void ManualReset()
        {
            // Apply the same penalties as the automatic reset
            SetPostFuelOutStats();
            outOfFuelFeedbacks?.PlayFeedbacks();
            AlertEvent.Trigger(AlertReason.OutOfFuel, "Manual reset activated!", "Reset to Base");

            // Trigger the reset event manually
            PlayerStatusEvent.Trigger(PlayerStatusEventType.OutOfFuel);
        }

// In PlayerDeathManager.cs
        public void SetPostFuelOutStats()
        {
            var maximumFuel = PlayerFuelManager.MaxFuelPoints;
            var recoveryAmount = fuelPenaltyMultiplier * maximumFuel;

            // Ensure a minimum amount (e.g., 10) if the penalty calculation is too low
            recoveryAmount = Mathf.Max(recoveryAmount, 10f);

            var maximumHealth = PlayerHealthManager.MaxHealthPoints;
            var recoveryHealth = healthPenaltyMultiplier * maximumHealth;

            var recoveryHealthAmount = Mathf.Max(recoveryHealth, 2f);

            // Use SetCurrentFuel to set the fuel amount
            FuelEvent.Trigger(FuelEventType.SetCurrentFuel, recoveryAmount, maximumFuel);
            HealthEvent.Trigger(HealthEventType.SetCurrentHealth, recoveryHealthAmount);
            CurrencyEvent.Trigger(CurrencyEventType.RemoveCurrency, GetRescueExpense());

            // Ensure the UI is updated
            FuelEvent.Trigger(FuelEventType.NotifyListeners, recoveryAmount, maximumFuel);

            SaveManager.Instance.SaveAll();

            // Debug the fuel reset
            UnityEngine.Debug.Log($"Set fuel to {recoveryAmount} after running out of fuel");
        }

        public float GetRescueExpense()
        {
            if (PlayerCurrencyManager.CompanyCredits < monetaryPenalty) return PlayerCurrencyManager.CompanyCredits;
            return monetaryPenalty;
        }

        private IEnumerator TriggerRevivalFeedbacks()
        {
            yield return new WaitForSeconds(2f);
            revivalFeedbacks?.PlayFeedbacks();
        }


        public void SetPostDeathStats()
        {
            var maximumHealth = PlayerHealthManager.MaxHealthPoints;
            var maxFuel = PlayerFuelManager.MaxFuelPoints;
            var currentCurrency = PlayerCurrencyManager.CompanyCredits;

            var recoveryFuel = fuelPenaltyMultiplier * maxFuel;
            recoveryFuel = Mathf.Max(recoveryFuel, 10f);

            var recoveryHealth = healthPenaltyMultiplier * maximumHealth;
            recoveryHealth = Mathf.Max(recoveryHealth, 2f);

            FuelEvent.Trigger(FuelEventType.SetCurrentFuel, recoveryFuel, maxFuel);
            HealthEvent.Trigger(HealthEventType.SetCurrentHealth, recoveryHealth);
            CurrencyEvent.Trigger(CurrencyEventType.RemoveCurrency, GetRescueExpense());

            // Ensure the UI is updated
            FuelEvent.Trigger(FuelEventType.NotifyListeners, recoveryFuel, maxFuel);

            SaveManager.Instance.SaveAll();

            // Add debug log
            UnityEngine.Debug.Log($"Set health to {recoveryHealth} and fuel to {recoveryFuel} after death");
        }

    }
}