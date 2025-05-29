using System;
using MoreMountains.Tools;
using UnityEngine;

namespace Domains.Player.Events
{
    public enum PlayerPositionEventType
    {
        ReportDepth
    }

    [Serializable]
    public struct PlayerPositionEvent
    {
        private static PlayerPositionEvent e;

        public PlayerPositionEventType EventType;
        public Vector3 Position;

        public static void Trigger(PlayerPositionEventType playerPositionEventType,
            Vector3 position)
        {
            e.EventType = playerPositionEventType;
            e.Position = position;
            MMEventManager.TriggerEvent(e);
        }
    }
}