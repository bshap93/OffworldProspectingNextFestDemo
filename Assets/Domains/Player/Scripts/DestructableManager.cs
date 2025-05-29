using System.Collections.Generic;
using Domains.Player.Events;
using Domains.Scene.Scripts;
using MoreMountains.Tools;
using UnityEditor;
using UnityEngine;

namespace Domains.Player.Scripts
{
#if UNITY_EDITOR

    public static class DestructableManagerDebug
    {
        [MenuItem("Debug/Reset Destructables")]
        public static void ResetDestructables()
        {
            DestructableManager.ResetDestructables();
        }
    }
#endif
    public class DestructableManager : MonoBehaviour, MMEventListener<DestructableEvent>, ICollectionManager
    {
        public static HashSet<string> DestructablesDestroyed = new();

        private Dictionary<string, bool> _destroyedOreNodeWasDestroyed;

        private string _savePath;

        private void Start()
        {
            _savePath = GetSaveFilePath();

            if (!HasSavedData())
            {
                UnityEngine.Debug.Log("[DestructableManager] No save file found, forcing initial save...");
                ResetDestructables(); // Ensure default values are set
            }

            LoadDestructables();
        }


        private void OnEnable()
        {
            this.MMEventStartListening();
        }

        private void OnDisable()
        {
            this.MMEventStopListening();
        }

        public void OnMMEvent(DestructableEvent eventType)
        {
            if (eventType.EventType == DestructableEventType.Destroyed)
            {
                UnityEngine.Debug.Log($"Destructable {eventType.UniqueID} was destroyed");
                AddDestructable(eventType.UniqueID, true);
            }
        }

        public static bool IsDestuctableDestroyed(string uniqueId)
        {
            return DestructablesDestroyed.Contains(uniqueId);
        }

        public void LoadDestructables()
        {
            if (_savePath == null) _savePath = GetSaveFilePath();

            if (ES3.FileExists(_savePath))
            {
                if (ES3.KeyExists("Destructables", _savePath))
                {
                    var loadedDestructables = ES3.Load<HashSet<string>>("Destructables", _savePath);
                    DestructablesDestroyed.Clear();

                    foreach (var destructable in loadedDestructables)
                    {
                        DestructablesDestroyed.Add(destructable);
                        UnityEngine.Debug.Log($"Loaded destructable: {destructable}");
                    }
                }
                else
                {
                    var keys = ES3.GetKeys(_savePath);
                    foreach (var key in keys)
                        if (ES3.KeyExists(key, _savePath) && ES3.Load<bool>(key, _savePath))
                        {
                            DestructablesDestroyed.Add(key);
                            UnityEngine.Debug.Log($"Loaded destructable: {key}");
                        }
                }
            }
        }

        public static void AddDestructable(string uniqueID, bool b)
        {
            if (b)
                DestructablesDestroyed.Add(uniqueID);

            UnityEngine.Debug.Log($"Item {uniqueID} marked as destroyed: {b}");

            // SaveAllDestructables();
        }

        public static void SaveAllDestructables()
        {
            var saveFilePath = GetSaveFilePath();

            ES3.Save("Destructables", DestructablesDestroyed, saveFilePath);

            foreach (var uniqueId in DestructablesDestroyed) ES3.Save(uniqueId, true, saveFilePath);
        }

        public static void ResetDestructables()
        {
            DestructablesDestroyed = new HashSet<string>();
        }

        public bool HasSavedData()
        {
            return ES3.FileExists(_savePath);
        }

        private static string GetSaveFilePath()
        {
            return SaveManager.SavePickablesFileName;
        }
    }
}