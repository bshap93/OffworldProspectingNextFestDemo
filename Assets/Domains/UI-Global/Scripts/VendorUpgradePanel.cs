using Domains.Player.Events;
using Domains.Player.Scripts;
using Domains.Player.Scripts.ScriptableObjects;
using MoreMountains.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Domains.Scripts
{
    public class VendorUpgradePanel : MonoBehaviour, MMEventListener<UpgradeEvent>
    {
        public UpgradeData upgradeData;

        public TMP_Text upgradeTypeNameText;
        public TMP_Text upgradeNameText;

        public TMP_Text nextUpgradeCostText;

        [SerializeField] private Image upgradeIconImage;

        [SerializeField] private Image cardBackgroundImage;

        private PlayerUpgradeManager _playerUpgradeManager;

        private int _upgradeLevel;

        private void Start()
        {
            _playerUpgradeManager = FindFirstObjectByType<PlayerUpgradeManager>();
            _upgradeLevel = _playerUpgradeManager.GetUpgradeLevel(upgradeData.upgradeTypeName);

            cardBackgroundImage = GetComponent<Image>();


            upgradeTypeNameText.text = upgradeData.upgradeTypeName; // Show upgrade category
            upgradeNameText.text =
                _playerUpgradeManager.GetUpgradeName(upgradeData.upgradeTypeName); // Show upgrade level name

            nextUpgradeCostText.text = _playerUpgradeManager.GetUpgradeCost(upgradeData.upgradeTypeName).ToString();
        }


        private void OnEnable()
        {
            this.MMEventStartListening();
        }

        private void OnDisable()
        {
            this.MMEventStopListening();
        }


        public void OnMMEvent(UpgradeEvent eventType)
        {
            if (eventType.EventType == UpgradeEventType.UpgradePurchased)
                // Update only if the upgrade type matches the current upgrade panel
                if (eventType.UpgradeData.upgradeTypeName == upgradeData.upgradeTypeName)
                {
                    upgradeNameText.text = _playerUpgradeManager.GetUpgradeName(eventType.UpgradeData.upgradeTypeName);

                    nextUpgradeCostText.text =
                        _playerUpgradeManager.GetUpgradeCost(eventType.UpgradeData.upgradeTypeName).ToString();
                }
        }

        public void TriggerUpgradePurchase()
        {
            _playerUpgradeManager.BuyUpgrade(upgradeData.upgradeTypeName);
        }
    }
}