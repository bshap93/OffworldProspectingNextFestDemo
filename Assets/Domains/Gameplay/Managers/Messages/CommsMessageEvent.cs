using System;
using MoreMountains.Tools;
using UnityEngine.Serialization;

namespace Domains.Gameplay.Managers.Messages
{
    [Serializable]
    public enum CommsMessageEventType
    {
        ReadMessage,
        SendMessage,
        UnreadMessageInit,
        ReadMessageInit
    }


    [Serializable]
    public struct CommsMessageEvent
    {
        private static CommsMessageEvent _e;

        [FormerlySerializedAs("MessageType")] public CommsMessageEventType messageType;

        [FormerlySerializedAs("CommsMessageId")]
        public string commsMessageId;

        public static void Trigger(string commsMessageId, CommsMessageEventType eventType)
        {
            _e.commsMessageId = commsMessageId;
            _e.messageType = eventType;


            MMEventManager.TriggerEvent(_e);
        }
    }
}