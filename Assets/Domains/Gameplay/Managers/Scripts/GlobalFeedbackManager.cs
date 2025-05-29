using Domains.Player.Events;
using Domains.Player.Scripts;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Serialization;

namespace Domains.Gameplay.Managers.Scripts
{
    public class GlobalFeedbackManager : MonoBehaviour, MMEventListener<UpgradeEvent>,
        MMEventListener<PlayerStatusEvent>, MMEventListener<CurrencyEvent>, MMEventListener<FuelEvent>
    {
        [FormerlySerializedAs("UpgradeFeedbacks")] [SerializeField]
        private MMFeedbacks upgradeFeedbacks;

        [SerializeField] private MMFeedbacks upgradeFailedFeedbacks;
        [SerializeField] private MMFeedbacks currencyAddedFeedbacks;
        [SerializeField] private MMFeedbacks lowOnFuelFeedbacks;


        private void OnEnable()
        {
            this.MMEventStartListening<UpgradeEvent>();
            this.MMEventStartListening<PlayerStatusEvent>();
            this.MMEventStartListening<CurrencyEvent>();
            this.MMEventStartListening<FuelEvent>();
        }

        private void OnDisable()
        {
            this.MMEventStopListening<UpgradeEvent>();
            this.MMEventStopListening<PlayerStatusEvent>();
            this.MMEventStopListening<CurrencyEvent>();
            this.MMEventStopListening<FuelEvent>();
        }

        public void OnMMEvent(CurrencyEvent eventType)
        {
            if (eventType.EventType == CurrencyEventType.AddCurrency)
                currencyAddedFeedbacks.PlayFeedbacks();
            else if (eventType.EventType == CurrencyEventType.RemoveCurrency)
                upgradeFailedFeedbacks.PlayFeedbacks();
        }

        public void OnMMEvent(FuelEvent eventType)
        {
            if (eventType.EventType == FuelEventType.LowOnFuel)
                lowOnFuelFeedbacks.PlayFeedbacks();
        }

        public void OnMMEvent(PlayerStatusEvent eventType)
        {
        }


        public void OnMMEvent(UpgradeEvent eventType)
        {
            if (eventType.EventType == UpgradeEventType.UpgradePurchased)
                upgradeFeedbacks.PlayFeedbacks();
            else if (eventType.EventType == UpgradeEventType.UpgradeFailed)
                upgradeFailedFeedbacks.PlayFeedbacks();
        }
    }
}