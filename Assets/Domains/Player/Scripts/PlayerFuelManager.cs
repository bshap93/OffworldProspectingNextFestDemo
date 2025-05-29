using Domains.Player.Events;
using Domains.Player.Scripts.ScriptableObjects;
using Domains.UI_Global;
using Domains.UI_Global.Events;
using MoreMountains.Tools;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Domains.Player.Scripts
{
#if UNITY_EDITOR
    public static class PlayerFuelManagerDebug
    {
        [MenuItem("Debug/Reset/Reset Fuel")]
        public static void ResetFuel()
        {
            PlayerFuelManager.ResetPlayerFuel();
        }
    }
#endif
    public class PlayerFuelManager : MonoBehaviour, MMEventListener<FuelEvent>
    {
        public static float FuelPoints;
        public static float MaxFuelPoints;


        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        public static float InitialCharacterFuel;

        [Header("Fuel Settings")] public static float lowAmount = 40f;


        [FormerlySerializedAs("AmountFuelReturnedMultiplier")]
        public float amountFuelReturnedMultiplier = 0.1f;

        [FormerlySerializedAs("staminaBarUpdater")]
        public FuelBarUpdater fuelBarUpdater;


        private string _savePath;
        private CharacterStatProfile characterStatProfile;


        private void Awake()
        {
            if (fuelBarUpdater == null)
            {
                fuelBarUpdater = FindFirstObjectByType<FuelBarUpdater>();
                if (fuelBarUpdater == null)
                    UnityEngine.Debug.LogError("PlayerFuelManager: No FuelBarUpdater found in scene!");
            }

            characterStatProfile =
                Resources.Load<CharacterStatProfile>(CharacterResourcePaths.CharacterStatProfileFilePath);
            if (characterStatProfile != null)
                InitialCharacterFuel = characterStatProfile.InitialFuel;
            else
                UnityEngine.Debug.LogError("CharacterStatProfile not set in PlayerFuelManager");
        }


        private void Start()
        {
            _savePath = GetSaveFilePath();

            if (!ES3.FileExists(_savePath))
            {
                UnityEngine.Debug.Log("[PlayerFuelManager] No save file found, forcing initial save...");
                ResetPlayerFuel(); // Ensure default values are set
            }

            LoadPlayerFuel();

            if (FuelPoints == 0)
            {
                var amtFuel = InitialCharacterFuel * amountFuelReturnedMultiplier;
                SetCurrentFuel(amtFuel); // Set to initial value if zero
                FuelEvent.Trigger(FuelEventType.NotifyListeners, amtFuel, MaxFuelPoints);
            }
        }


        private void OnEnable()
        {
            this.MMEventStartListening();
        }

        private void OnDisable()
        {
            this.MMEventStopListening();
        }

        public void OnMMEvent(FuelEvent fuelEvent)
        {
            switch (fuelEvent.EventType)
            {
                case FuelEventType.ConsumeFuel:
                    ConsumeFuel(fuelEvent.CurrentByValue);
                    break;
                case FuelEventType.RecoverFuel:
                    RecoverFuel(fuelEvent.CurrentByValue);
                    break;
                case FuelEventType.FullyRecoverFuel:
                    FullyRecoverFuel();
                    break;
                case FuelEventType.IncreaseMaximumFuel:
                    IncreaseMaximumFuel(fuelEvent.CurrentByValue);
                    break;
                case FuelEventType.SetCurrentFuel:
                    SetCurrentFuel(fuelEvent.CurrentByValue);
                    break;
                case FuelEventType.SetMaxFuel:
                    FuelPoints = fuelEvent.CurrentByValue;
                    break;
            }
        }

        private void SetCurrentFuel(float value)
        {
            FuelPoints = value;
            SavePlayerFuel();
        }

        public void Initialize()
        {
            ResetPlayerFuel();
            fuelBarUpdater.Initialize();
        }

// In PlayerFuelManager.cs
        public static void ConsumeFuel(float amount)
        {
            if (FuelPoints - amount <= 0)
            {
                // Player is out of fuel
                PlayerStatusEvent.Trigger(PlayerStatusEventType.OutOfFuel);
                AlertEvent.Trigger(AlertReason.OutOfFuel, "You are out of fuel!", "Out of Fuel");
                FuelPoints = 0; // Set to zero for consistent state

                // Note: Don't worry about recovery here, let the PlayerDeathManager handle that
            }
            else if (FuelPoints - amount <= lowAmount && FuelPoints > lowAmount)
            {
                // Player is low on fuel
                FuelEvent.Trigger(FuelEventType.LowOnFuel, FuelPoints, MaxFuelPoints);
                AlertEvent.Trigger(AlertReason.LowOnFuel, "You are low on fuel!", "Low Fuel");

                FuelPoints -= amount;
            }
            else
            {
                FuelPoints -= amount;
            }

            // After changing the value, trigger an event to update UI
            FuelEvent.Trigger(FuelEventType.NotifyListeners, FuelPoints, MaxFuelPoints);
            SavePlayerFuel();
        }

        public static void RecoverFuel(float amount)
        {
            if (FuelPoints == 0 && amount > 0) PlayerStatusEvent.Trigger(PlayerStatusEventType.RegainedFuel);
            FuelPoints += amount;

            // After changing the value, trigger an event to update UI
            FuelEvent.Trigger(FuelEventType.NotifyListeners, FuelPoints, MaxFuelPoints);
            SavePlayerFuel();
        }

        public static void FullyRecoverFuel()
        {
            FuelPoints = MaxFuelPoints;
            PlayerStatusEvent.Trigger(PlayerStatusEventType.RegainedFuel);

            FuelEvent.Trigger(FuelEventType.NotifyListeners, FuelPoints, MaxFuelPoints);
            SavePlayerFuel();
        }

        public static void IncreaseMaximumFuel(float amount)
        {
            MaxFuelPoints += amount;
        }

        public static void DecreaseMaximumFuel(float amount)
        {
            MaxFuelPoints -= amount;
        }

        private static string GetSaveFilePath()
        {
            return "GameSave.es3"; // Always use the same file
        }

        public void LoadPlayerFuel()
        {
            var saveFilePath = GetSaveFilePath();

            if (ES3.FileExists(saveFilePath))
            {
                FuelPoints = ES3.Load<float>("FuelPoints", saveFilePath);
                MaxFuelPoints = ES3.Load<float>("MaxFuelPoints", saveFilePath);
                fuelBarUpdater.Initialize();
                // UnityEngine.Debug.Log(
                //     $"✅ Loaded fuel data: Fuel Points={FuelPoints}, Max Fuel Points={MaxFuelPoints}");
            }
            else
            {
                UnityEngine.Debug.Log($"❌ No saved fuel data found at {saveFilePath}");

                ResetPlayerFuel();
                fuelBarUpdater.Initialize();
            }
        }

        public static void ResetPlayerFuel()
        {
            var characterStatProfile =
                Resources.Load<CharacterStatProfile>(CharacterResourcePaths.CharacterStatProfileFilePath);

            if (characterStatProfile == null)
            {
                UnityEngine.Debug.LogError("\u274c CharacterStatProfile not found! Using default values.");
                FuelPoints = 100f;
                MaxFuelPoints = 100f;
            }
            else
            {
                FuelPoints = characterStatProfile.InitialFuel;
                MaxFuelPoints = characterStatProfile.InitialMaxFuel;
            }

            PlayerStatusEvent.Trigger(PlayerStatusEventType.ResetFuel);
        }

        public static void SavePlayerFuel()
        {
            ES3.Save("FuelPoints", FuelPoints, "GameSave.es3");
            ES3.Save("MaxFuelPoints", MaxFuelPoints, "GameSave.es3");
        }

        // In PlayerFuelManager.cs
        public static bool IsPlayerStranded()
        {
            // Player is stranded if they're out of fuel and teleportWhenOutOfFuel is false
            return IsPlayerOutOfFuel() && FindFirstObjectByType<PlayerDeathManager>()?.autoResetWhenOutOfFuel == false;
        }

        public bool HasSavedData()
        {
            return ES3.FileExists(GetSaveFilePath());
        }

        public static bool IsPlayerOutOfFuel()
        {
            return FuelPoints <= 0;
        }

        // Add this to PlayerFuelManager.cs
        public static void EnsureMinimumFuel(float minimumAmount = 10f)
        {
            if (FuelPoints < minimumAmount)
            {
                FuelPoints = minimumAmount;
                FuelEvent.Trigger(FuelEventType.NotifyListeners, FuelPoints, MaxFuelPoints);
                SavePlayerFuel();
                UnityEngine.Debug.Log($"Ensuring minimum fuel of {minimumAmount}");
            }
        }
    }
}