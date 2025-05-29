using System.Collections.Generic;
using Domains.Scene.Scripts;
using Gameplay.Events;
using MoreMountains.Tools;
using UnityEditor;
using UnityEngine;

namespace Domains.Player.Scripts
{
#if UNITY_EDITOR
    public static class PickableManagerDebug
    {
        [MenuItem("Debug/Reset Picked Items")]
        public static void ResetPickedItemsMenu()
        {
            PickableManager.ResetPickedItems();
        }
    }
#endif


    public class PickableManager : MonoBehaviour, MMEventListener<ItemEvent>
    {
        public static HashSet<string> PickedItems = new();

        private Dictionary<string, bool> _pickedItemIdWasPicked;

        private string _savePath;


        private void Start()
        {
            _savePath = GetSaveFilePath();

            if (!HasSavedData())
            {
                UnityEngine.Debug.Log("[PickableManager] No save file found, forcing initial save...");
                ResetPickedItems(); // Ensure default values are set
            }

            LoadPickedItems();
        }


        private void OnEnable()
        {
            this.MMEventStartListening();
        }

        private void OnDisable()
        {
            this.MMEventStopListening();
        }

        public void OnMMEvent(ItemEvent eventType)
        {
            if (eventType.EventType == ItemEventType.Picked)
            {
                AddPickedItem(eventType.Item.uniqueID, true);
                UnityEngine.Debug.Log($"Item picked: {eventType.Item.BaseItem.ItemName}");
            }
        }

        private static string GetSaveFilePath()
        {
            return SaveManager.SavePickablesFileName;
        }

        public void LoadPickedItems()
        {
            if (_savePath == null)
                _savePath = GetSaveFilePath();

            if (ES3.FileExists(_savePath))
            {
                // Check if we have the entire HashSet saved under "PickedItems" key
                if (ES3.KeyExists("PickedItems", _savePath))
                {
                    // Load the entire HashSet at once
                    var loadedItems = ES3.Load<HashSet<string>>("PickedItems", _savePath);
                    PickedItems.Clear();

                    foreach (var item in loadedItems) PickedItems.Add(item);
                }
                else
                {
                    // Fallback to the old method of loading individual keys
                    var keys = ES3.GetKeys(_savePath);
                    foreach (var key in keys)
                        if (ES3.KeyExists(key, _savePath) && ES3.Load<bool>(key, _savePath))
                        {
                            UnityEngine.Debug.Log("Loaded picked item: " + key);
                            PickedItems.Add(key);
                        }
                }
            }
        }

        public static void ResetPickedItems()
        {
            PickedItems = new HashSet<string>();
        }


        public static bool IsItemPicked(string uniqueID)
        {
            return PickedItems.Contains(uniqueID);
        }

        public static void AddPickedItem(string uniqueID, bool b)
        {
            if (b)
                PickedItems.Add(uniqueID);

            UnityEngine.Debug.Log($"Item {uniqueID} marked as picked: {b}");

            // SaveAllPickedItems();
        }


        public static void SaveAllPickedItems()
        {
            var saveFilePath = GetSaveFilePath();

            // Save both the full HashSet and individual items
            ES3.Save("PickedItems", PickedItems, saveFilePath);

            // Also save individual items for backwards compatibility
            foreach (var uniqueID in PickedItems)
                ES3.Save(uniqueID, true, saveFilePath);
        }

        public bool HasSavedData()
        {
            return ES3.FileExists(_savePath);
        }
    }
}