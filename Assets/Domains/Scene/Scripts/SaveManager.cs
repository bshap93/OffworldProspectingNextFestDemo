using System;
using System.Collections;
using Domains.Gameplay.Managers.Messages;
using Domains.Gameplay.Objectives.Scripts;
using Domains.Player.Progression;
using Domains.Player.Scripts;
using Domains.SaveLoad;
using Domains.UI_Global.Events;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Serialization;

namespace Domains.Scene.Scripts
{
    [Serializable]
    public class SaveManager : MonoBehaviour, MMEventListener<SaveLoadEvent>
    {
        public const string SaveFileName = "GameSave.es3";
        public const string SavePickablesFileName = "Pickables.es3";
        public const string SaveProgressionFilePath = "Progression.es3";
        public const string SaveObjectivesFilePath = "Objectives.es3";
        public const string SaveMessagesFilePath = "Messages.es3";


        // [Header("Persistence Managers")] [SerializeField]
        // InventoryPersistenceManager inventoryManager;
        [FormerlySerializedAs("playerStaminaManager")]
        [FormerlySerializedAs("playerMutableStatsManager")]
        [FormerlySerializedAs("playerStatsManager")]
        [FormerlySerializedAs("resourcesManager")]
        [SerializeField]
        private PlayerFuelManager playerFuelManager;

        [SerializeField] private PlayerHealthManager playerHealthManager;

        [SerializeField] private MMFeedbacks saveFeedbacks;

        [Header("Item & Container Persistence")]
        public PickableManager pickableManager;

        public PlayerInventoryManager playerInventoryManager;

        public PlayerCurrencyManager playerCurrencyManager;

        public PlayerUpgradeManager playerUpgradeManager;

        public DestructableManager destructableManager;

        public ProgressionManager progressionManager;

        public ObjectivesManager objectivesManager;

        public MessageManager messageManager;


        public static SaveManager Instance { get; private set; }


        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }


        private void OnEnable()
        {
            this.MMEventStartListening();
        }

        private void OnDisable()
        {
            this.MMEventStopListening();
        }


        // On App Quit
        private void OnApplicationQuit()
        {
            StartCoroutine(SaveAllThenWait());
        }

        public void OnMMEvent(SaveLoadEvent eventType)
        {
            if (eventType.EventType == SaveLoadEventType.Save)
                SaveAll();
            else if (eventType.EventType == SaveLoadEventType.Load) LoadAll();
        }


        private string GetSaveFileName()
        {
            return "GameSave.es3"; // Always use a single save file
        }

        public void SaveAll()
        {
            PlayerFuelManager.SavePlayerFuel();
            PlayerHealthManager.SavePlayerHealth();
            if (playerInventoryManager != null)
                PlayerInventoryManager.SaveInventory();
            else
                UnityEngine.Debug.LogError("PlayerInventoryManager.Instance is null. Skipping inventory save.");
            PlayerCurrencyManager.SavePlayerCurrency();
            PlayerUpgradeManager.SaveUpgrades();
            PickableManager.SaveAllPickedItems();
            DestructableManager.SaveAllDestructables();
            ObjectivesManager.SaveAllObjectives();
            MessageManager.SaveAllMessages();


            ProgressionManager.SaveAllProgression(false);

            // UnityEngine.Debug.Log("All data saved");

            AlertEvent.Trigger(AlertReason.SavingGame, "Saving game...");
        }

        public bool LoadAll()
        {
            var fuelLoaded = playerFuelManager != null && playerFuelManager.HasSavedData();
            var healthLoaded = playerHealthManager != null && playerHealthManager.HasSavedData();
            var inventoryLoaded = playerInventoryManager != null && playerInventoryManager.HasSavedData();
            var currencyLoaded = playerCurrencyManager != null && playerCurrencyManager.HasSavedData();
            var upgradesLoaded = playerUpgradeManager != null && playerUpgradeManager.HasSavedData();
            var pickablesLoaded = pickableManager != null && pickableManager.HasSavedData();
            var destructablesLoaded = destructableManager != null && destructableManager.HasSavedData();
            var progressionLoaded = progressionManager != null && progressionManager.HasProgressionData();
            var objectivesLoaded = objectivesManager != null && objectivesManager.HasSavedData();
            var messagesLoaded = messageManager != null && messageManager.HasSavedData();


            // Digger has no Load method

            if (fuelLoaded) playerFuelManager.LoadPlayerFuel();
            if (healthLoaded) playerHealthManager.LoadPlayerHealth();
            if (inventoryLoaded) playerInventoryManager.LoadInventory();
            if (currencyLoaded) playerCurrencyManager.LoadPlayerCurrency();
            if (upgradesLoaded) playerUpgradeManager.LoadUpgrades();
            if (pickablesLoaded) pickableManager.LoadPickedItems();
            if (destructablesLoaded) destructableManager.LoadDestructables();
            if (progressionLoaded) progressionManager.LoadProgressionObjectivesState();
            if (objectivesLoaded) objectivesManager.LoadObjectives();
            if (messagesLoaded) messageManager.LoadMessages();


            // 3rd Party


            return fuelLoaded ||
                   healthLoaded || inventoryLoaded || currencyLoaded ||
                   upgradesLoaded || pickablesLoaded || destructablesLoaded || progressionLoaded ||
                   objectivesLoaded || messagesLoaded;
        }

        public void CallSaveThenWait()
        {
            StartCoroutine(SaveAllThenWait());
        }

        private IEnumerator SaveAllThenWait()
        {
            SaveAll();
            saveFeedbacks?.PlayFeedbacks();
            AlertEvent.Trigger(AlertReason.SavingGame, "Saving game...");
            yield return new WaitForSeconds(1);
        }
    }
}