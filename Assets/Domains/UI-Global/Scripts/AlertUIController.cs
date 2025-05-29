using Domains.UI_Global.Events;
using Michsky.MUIP;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using UnityEngine;

namespace Domains.Scripts_that_Need_Sorting
{
    public class AlertUIController : MonoBehaviour, MMEventListener<AlertEvent>
    {
        [SerializeField] private MMFeedbacks normalAlertFeedbacks;
        private NotificationManager _notificationManager;

        private void Awake()
        {
            _notificationManager = GetComponentInChildren<NotificationManager>();
        }

        private void Start()
        {
            AlertEvent.Trigger(AlertReason.Test,
                "Use the Toggle Locations button to turn location indicators off and on",
                "Locations");
        }

        private void OnEnable()
        {
            this.MMEventStartListening();
        }

        private void OnDisable()
        {
            this.MMEventStopListening();
        }

        public void OnMMEvent(AlertEvent eventType)
        {
            if (eventType.AlertType == AlertType.Basic)
                ShowBasicAlert(eventType);
        }


        public void ShowBasicAlert(AlertEvent evt)
        {
            _notificationManager.title = evt.AlertTitle;
            _notificationManager.description = evt.AlertMessage;
            _notificationManager.icon = evt.AlertIcon;
            _notificationManager.UpdateUI();


            _notificationManager.Open();
        }

        public void HideAlert()
        {
            _notificationManager.Close();
        }
    }
}