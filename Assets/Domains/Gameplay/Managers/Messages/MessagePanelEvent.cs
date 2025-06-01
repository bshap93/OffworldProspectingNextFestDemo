using System;
using MoreMountains.Tools;

namespace Domains.Gameplay.Managers.Messages
{
    [Serializable]
    public enum MessagePanelEventType
    {
        MessagePanelOpened,
        MessagePanelClosed
    }

    [Serializable]
    public struct MessagePanelEvent
    {
        private static MessagePanelEvent _e;

        public string messagePanelId;

        public MessagePanelEventType messagePanelEventType;

        public static void Trigger(string messagePanelId, MessagePanelEventType eventType)
        {
            _e.messagePanelId = messagePanelId;
            _e.messagePanelEventType = eventType;
            MMEventManager.TriggerEvent(_e);
        }
    }
}