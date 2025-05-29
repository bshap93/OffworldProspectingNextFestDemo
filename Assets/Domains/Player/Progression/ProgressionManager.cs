using System.Collections.Generic;
using Domains.Gameplay.Managers;
using Domains.Player.Events;
using MoreMountains.Tools;
using UnityEditor;
using UnityEngine;

namespace Domains.Player.Progression
{
#if UNITY_EDITOR

    public static class ProgressionManagerDebug
    {
        [MenuItem("Debug/Reset Progression")]
        public static void ResetProgressionObjectives()
        {
            ProgressionManager.ResetProgression();
        }
    }
#endif
    [DefaultExecutionOrder(-10)]
    public class ProgressionManager : Manager, MMEventListener<ProgressionEvent>
    {
        private const string ObjectivesKeyName = "CollectableObjectives";
        private const string TutorialFinishedKeyName = "TutorialFinished";
        public static HashSet<string> CollectedObjectives = new();
        public static bool TutorialFinished = true;

        public static bool IsNewGame;


        private string _collectedObjectiveSave;

        private Dictionary<string, bool> _progressionObjectivesWasCollected;

        private string _savePath;
        public static bool IsInitialized { get; }


        private void Start()
        {
            _collectedObjectiveSave = GetSaveFilePath();

            if (!HasProgressionData())
            {
                UnityEngine.Debug.Log("[ProgressionManager] No save file found, forcing initial save...");
                ResetProgression(); // Ensure default values are set
            }


            LoadProgression();

            if (!TutorialFinished)
                ProgressionEvent.Trigger(ProgressionEventType.StartTutorial);
            else
                ProgressionEvent.Trigger(ProgressionEventType.FinishTutorial);
        }


        private void OnEnable()
        {
            this.MMEventStartListening();
        }

        private void OnDisable()
        {
            this.MMEventStopListening();
        }

        public void OnMMEvent(ProgressionEvent eventType)
        {
            if (eventType.EventType == ProgressionEventType.FinishTutorial) TutorialFinished = true;

            if (eventType.EventType == ProgressionEventType.StartTutorial) TutorialFinished = false;

            if (eventType.EventType == ProgressionEventType.CollectedObjective)
                AddInteractableObjective(eventType.UniqueID, true);
        }

        public static void FinishTutorial()
        {
            TutorialFinished = true;
            SaveAllProgression(false);

            UnityEngine.Debug.Log("ProgressionManager: Tutorial finished");
        }

        private void LoadProgression()
        {
            if (!HasProgressionData()) return;
            LoadBooleanFlags();

            LoadProgressionObjectivesState();

            if (CollectedObjectives.Count > 0)
                UnityEngine.Debug.Log($"Loaded objectives: {CollectedObjectives.Count}");
        }

        public static void AddInteractableObjective(string uniqueId, bool wasCollected)
        {
            if (wasCollected) CollectedObjectives.Add(uniqueId);

            // UnityEngine.Debug.Log($"ProgressionManager: {uniqueId} was collected");
        }

        public static bool IsObjectiveCollected(string uniqueId)
        {
            return CollectedObjectives.Contains(uniqueId);
        }


        public static void ResetProgression()
        {
            CollectedObjectives = new HashSet<string>();
            TutorialFinished = false;
            IsNewGame = true;
        }

        public static void ContinueGame()
        {
            IsNewGame = false;
            UnityEngine.Debug.Log("ProgressionManager: Continue game");
        }


        public bool HasProgressionData()
        {
            return ES3.FileExists(_collectedObjectiveSave) &&
                   ES3.KeyExists(ObjectivesKeyName, _collectedObjectiveSave) &&
                   ES3.KeyExists(TutorialFinishedKeyName, _collectedObjectiveSave);
        }

        protected override void LoadBooleanFlags()
        {
            if (_savePath == null) _savePath = GetSaveFilePath();

            if (ES3.KeyExists(TutorialFinishedKeyName, _savePath))
            {
                TutorialFinished = ES3.Load<bool>(TutorialFinishedKeyName, _savePath);
            }
            else
            {
                TutorialFinished = false;
                UnityEngine.Debug.Log($"No saved tutorial finished state found. Defaulting to: {TutorialFinished}");
            }
        }


        public void LoadProgressionObjectivesState()
        {
            if (_savePath == null) _savePath = GetSaveFilePath();

            if (ES3.KeyExists(ObjectivesKeyName, _savePath))
            {
                var loadedObjectives = ES3.Load<HashSet<string>>(ObjectivesKeyName, _savePath);
                CollectedObjectives.Clear();

                foreach (var objective in loadedObjectives)
                {
                    CollectedObjectives.Add(objective);
                    UnityEngine.Debug.Log($"Loaded collectable objective: {objective}");
                }
            }
            else
            {
                var keys = ES3.GetKeys(_savePath);
                foreach (var key in keys)
                    if (ES3.KeyExists(key, _savePath) && ES3.Load<bool>(key, _savePath))
                    {
                        CollectedObjectives.Add(key);
                        UnityEngine.Debug.Log($"Loaded collectable objective: {key}");
                    }
            }
        }

        public static void SaveAllProgression(bool newGame)
        {
            var savePath = GetSaveFilePath();

            if (newGame) IsNewGame = true;
            ES3.Save("IsNewGame", IsNewGame, savePath);

            ES3.Save(ObjectivesKeyName, CollectedObjectives, savePath);
            ES3.Save(TutorialFinishedKeyName, TutorialFinished, savePath);

            foreach (var uniqueId in CollectedObjectives) ES3.Save(uniqueId, true, savePath);
        }
    }
}