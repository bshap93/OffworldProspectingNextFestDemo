using Domains.Input.Scripts;
using Domains.Player.Events;
using Domains.Player.Scripts.ScriptableObjects;
using Domains.Scene.Scripts;
using Domains.UI_Global.Events;
using Domains.UI_Global.UIUpdaters;
using MoreMountains.Tools;
using UnityEditor;
using UnityEngine;

namespace Domains.Player.Scripts
{
#if UNITY_EDITOR
    public static class PlayerCurrencyManagerDebug
    {
        [MenuItem("Debug/Reset/Reset Currency")]
        public static void ResetCurrency()
        {
            PlayerCurrencyManager.ResetPlayerCurrency();
        }
    }
#endif

    public class PlayerCurrencyManager : MonoBehaviour, MMEventListener<CurrencyEvent>
    {
        public static float CompanyCredits;
        public static float InitialCurrencyAmount;

        public CurrencyBarUpdater currencyBarUpdater;

        private string _savePath;
        private CharacterStatProfile characterStatProfile;

        private void Awake()
        {
            if (currencyBarUpdater == null)
            {
                currencyBarUpdater = FindFirstObjectByType<CurrencyBarUpdater>();
                if (currencyBarUpdater == null)
                    UnityEngine.Debug.LogError("PlayerCurrencyManager: No CurrencyBarUpdater found in scene!");
            }

            characterStatProfile =
                Resources.Load<CharacterStatProfile>(CharacterResourcePaths.CharacterStatProfileFilePath);
            if (characterStatProfile != null)
                InitialCurrencyAmount = characterStatProfile.InitialCurrency;
            else
                UnityEngine.Debug.LogError("CharacterStatProfile not set in PlayerCurrencyManager");
        }

        private void Start()
        {
            _savePath = GetSaveFilePath();

            if (!ES3.FileExists(_savePath))
            {
                UnityEngine.Debug.Log("[PlayerCurrencyManager] No save file found, forcing initial save...");
                ResetPlayerCurrency(); // Ensure default values are set
            }

            LoadPlayerCurrency();
        }

        private void Update()
        {
            if (CustomInputBindings.IsSaveDebugKeyPressed()) // Press F5 to force save
            {
                SavePlayerCurrency();
                UnityEngine.Debug.Log("Player currency saved");
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

        public void OnMMEvent(CurrencyEvent eventType)
        {
            switch (eventType.EventType)
            {
                case CurrencyEventType.AddCurrency:
                    AddCurrency(eventType.Amount);
                    break;
                case CurrencyEventType.RemoveCurrency:
                    RemoveCurrency(eventType.Amount);
                    break;

                case CurrencyEventType.SetCurrency:
                    SetCurrency(eventType.Amount);
                    break;
                default:
                    UnityEngine.Debug.LogWarning($"Unknown CurrencyEventType: {eventType.EventType}");
                    break;
            }
        }

        public void Initialize()
        {
            ResetPlayerCurrency();
            currencyBarUpdater.Initialize();
        }

        public static void AddCurrency(float amount)
        {
            CompanyCredits += amount;
            // Add an event trigger to notify UI and other systems
        }

        public static void LoseCurrency(float amount)
        {
            if (CompanyCredits - amount < 0)
                CompanyCredits = 0;
            else
                CompanyCredits -= amount;

            SavePlayerCurrency();
        }

        public static void RemoveCurrency(float amount)
        {
            if (CompanyCredits - amount < 0)
            {
                CompanyCredits = 0;
                AlertEvent.Trigger(AlertReason.InsufficientFunds,
                    "You don't have enough funds to complete this action.",
                    "Insufficient Funds");
            }
            else
            {
                CompanyCredits -= amount;
            }
        }

        public static void SetCurrency(float amount)
        {
            CompanyCredits = amount;
        }

        private static string GetSaveFilePath()
        {
            return SaveManager.SaveFileName;
        }

        public void LoadPlayerCurrency()
        {
            var saveFilePath = GetSaveFilePath();

            if (ES3.FileExists(saveFilePath) && ES3.KeyExists("CompanyCredits", saveFilePath))
            {
                CompanyCredits = ES3.Load<float>("CompanyCredits", saveFilePath);
                currencyBarUpdater.Initialize();
            }
            else
            {
                ResetPlayerCurrency();
                currencyBarUpdater.Initialize();
            }
        }

        public static void ResetPlayerCurrency()
        {
            var characterStatProfile =
                Resources.Load<CharacterStatProfile>(CharacterResourcePaths.CharacterStatProfileFilePath);

            if (characterStatProfile == null)
            {
                UnityEngine.Debug.LogError("CharacterStatProfile not found! Using default values.");
                CompanyCredits = 0; // Default fallback
            }
            else
            {
                CompanyCredits = characterStatProfile.InitialCurrency;
            }
        }

        public static void SavePlayerCurrency()
        {
            ES3.Save("CompanyCredits", CompanyCredits, "GameSave.es3");
        }

        public bool HasSavedData()
        {
            return ES3.FileExists(GetSaveFilePath()) &&
                   ES3.KeyExists("CompanyCredits", GetSaveFilePath());
        }
    }
}