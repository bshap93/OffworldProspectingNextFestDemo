using System;
using JetBrains.Annotations;
using MoreMountains.Tools;
using UnityEngine;

namespace Domains.UI_Global.Events
{
    [Serializable]
    public enum AlertReason
    {
        InventoryFull,
        Test,
        InsufficientFunds,
        OutOfFuel,
        SavingGame,
        DeletingDiggerData,
        Died,
        CreditsAdded,
        LowOnFuel,
        ResetManually,
        InventotryEmpty,
        HealthFull,
        PickedUpEquipment,
        DiggerPersisted
    }

    public enum AlertType
    {
        Basic,
        CallToAction
    }

    public struct AlertEvent
    {
        public static AlertEvent _e;

        public AlertType AlertType;

        public AlertReason AlertReason;
        public string AlertMessage;
        [CanBeNull] public string AlertTitle;
        [CanBeNull] public Sprite AlertIcon;
        [CanBeNull] public AudioClip AlertSound;
        [CanBeNull] public Color AlertColor;

        public static void Trigger(AlertReason alertReason, string alertMessage, string alertTitle = "Alert",
            Sprite alertIcon = null,
            AudioClip alertSound = null, Color alertColor = default, AlertType alertType = AlertType.Basic)
        {
            _e.AlertReason = alertReason;
            _e.AlertMessage = alertMessage;
            _e.AlertTitle = alertTitle;
            _e.AlertIcon = alertIcon;
            _e.AlertType = alertType;
            MMEventManager.TriggerEvent(_e);
        }
    }
}