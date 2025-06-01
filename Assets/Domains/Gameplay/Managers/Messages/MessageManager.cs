using System.Collections;
using System.Collections.Generic;
using Domains.Player.Scripts;
using Domains.Scene.Scripts;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using UnityEngine;

namespace Domains.Gameplay.Managers.Messages
{
    public class MessageManager : MonoBehaviour, ICollectionManager, MMEventListener<CommsMessageEvent>,
        MMEventListener<MessagePanelEvent>
    {
        public static HashSet<string> PanelMessagesRead = new();

        public static HashSet<string> ReadCommsMessages = new();

        public static HashSet<string> UnreadMessages = new();

        public static HashSet<string> AllMessages = new();

        public MMFeedbacks readMessageFeedback;
        public MMFeedbacks sendMessageFeedback;

        public CommsMessagesList commsMessagesList;

        private string _savePath;

        private void Start()
        {
            _savePath = GetSaveFilePath();

            if (!HasSavedData())
            {
                UnityEngine.Debug.Log("[MessageManager] No save file found, forcing initial save...");
                ResetMessages(); // Ensure default values are set
            }

            foreach (var commsMessageObject in commsMessagesList.commsMessages)
                AllMessages.Add(commsMessageObject.commsMessageId);

            LoadMessages();

            StartCoroutine(InitializeAfterFrame());
        }

        private void OnEnable()
        {
            this.MMEventStartListening<CommsMessageEvent>();
            this.MMEventStartListening<MessagePanelEvent>();
        }

        private void OnDisable()
        {
            this.MMEventStopListening<CommsMessageEvent>();
            this.MMEventStopListening<MessagePanelEvent>();
        }

        public void OnMMEvent(CommsMessageEvent eventType)
        {
            var commsMessageId = eventType.commsMessageId;

            if (eventType.messageType == CommsMessageEventType.ReadMessage) ReadUnreadMessage(eventType.commsMessageId);

            if (eventType.messageType == CommsMessageEventType.SendMessage) SendCommsMessage(eventType.commsMessageId);
        }

        public void OnMMEvent(MessagePanelEvent eventType)
        {
            var messagePanelId = eventType.messagePanelId;

            if (eventType.messagePanelEventType == MessagePanelEventType.MessagePanelOpened)
                PanelMessagesRead.Add(messagePanelId);
        }

        private IEnumerator InitializeAfterFrame()
        {
            yield return null;

            foreach (var commsMessage in UnreadMessages)
                CommsMessageEvent.Trigger(commsMessage, CommsMessageEventType.UnreadMessageInit);

            foreach (var commsMessage in ReadCommsMessages)
                CommsMessageEvent.Trigger(commsMessage, CommsMessageEventType.ReadMessageInit);

            SaveAllMessages();
        }

        public static bool IsCommsMessageRead(string commsMessageId)
        {
            return ReadCommsMessages.Contains(commsMessageId);
        }

        public static bool IsCommsMessageUnread(string commsMessageId)
        {
            return UnreadMessages.Contains(commsMessageId);
        }

        public static bool IsPanelMessageRead(string messagePanelId)
        {
            return PanelMessagesRead.Contains(messagePanelId);
        }

        public void LoadMessages()
        {
            if (_savePath == null) _savePath = GetSaveFilePath();

            if (ES3.KeyExists("ReadCommsMessages", _savePath))
            {
                var readMessages = ES3.Load<HashSet<string>>("ReadCommsMessages", _savePath);
                ReadCommsMessages.Clear();

                foreach (var message in readMessages) ReadCommsMessages.Add(message);
            }

            if (ES3.KeyExists("UnreadCommsMessages", _savePath))
            {
                var unreadMessages = ES3.Load<HashSet<string>>("UnreadCommsMessages", _savePath);
                UnreadMessages.Clear();

                foreach (var message in unreadMessages) UnreadMessages.Add(message);
            }

            if (ES3.KeyExists("PanelMessagesRead", _savePath))
            {
                var panelMessages = ES3.Load<HashSet<string>>("PanelMessagesRead", _savePath);
                PanelMessagesRead.Clear();

                foreach (var message in panelMessages) PanelMessagesRead.Add(message);
            }
        }

        public void ReadUnreadMessage(string commsMessageId)
        {
            if (!AllMessages.Contains(commsMessageId))
            {
                UnityEngine.Debug.LogWarning(
                    $"[MessageManager] Attempted to read a non-existent message: {commsMessageId}");
                return;
            }

            if (UnreadMessages.Contains(commsMessageId))
            {
                UnreadMessages.Remove(commsMessageId);
                ReadCommsMessages.Add(commsMessageId);
                readMessageFeedback?.PlayFeedbacks();
            }
        }

        public void SendCommsMessage(string commsMessageId)
        {
            if (!AllMessages.Contains(commsMessageId))
            {
                UnityEngine.Debug.LogWarning("[MessageManager] Attempted to send a non-existent message: " +
                                             commsMessageId);
                return;
            }

            if (!UnreadMessages.Contains(commsMessageId) && !ReadCommsMessages.Contains(commsMessageId))
            {
                UnreadMessages.Add(commsMessageId);
                sendMessageFeedback?.PlayFeedbacks();
            }
        }

        public static void SaveAllMessages()
        {
            var savePath = GetSaveFilePath();

            ES3.Save("ReadCommsMessages", ReadCommsMessages, savePath);
            ES3.Save("UnreadCommsMessages", UnreadMessages, savePath);
            ES3.Save("PanelMessagesRead", PanelMessagesRead, savePath);
        }

        public bool HasSavedData()
        {
            return ES3.FileExists(_savePath);
        }

        public static void ResetMessages()
        {
            UnreadMessages = new HashSet<string>();
            ReadCommsMessages = new HashSet<string>();
            PanelMessagesRead = new HashSet<string>();
        }

        private static string GetSaveFilePath()
        {
            return SaveManager.SaveMessagesFilePath;
        }

        public CommsMessageObject GetCommsMessageById(string commsMessageId)
        {
            foreach (var commsMessage in commsMessagesList.commsMessages)
                if (commsMessage.commsMessageId == commsMessageId)
                    return commsMessage;

            UnityEngine.Debug.LogWarning($"[MessageManager] Comms message with ID {commsMessageId} not found.");
            return null;
        }

        public static void ReadAllCommsMessages()
        {
            foreach (var objectiveId in UnreadMessages)
                if (!ReadCommsMessages.Contains(objectiveId))
                    CommsMessageEvent.Trigger(objectiveId, CommsMessageEventType.ReadMessage);

            SaveAllMessages();
        }

        // public void ReadPanelMessage(string messagePanelId)
        // {
        //
        // }
    }
}