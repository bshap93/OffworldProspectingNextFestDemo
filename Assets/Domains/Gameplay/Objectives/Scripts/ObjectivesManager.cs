using System.Collections;
using System.Collections.Generic;
using Domains.Gameplay.Objectives.Events;
using Domains.Gameplay.Objectives.ScriptableObjects;
using Domains.Player.Scripts;
using Domains.Scene.Scripts;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using UnityEngine;

namespace Domains.Gameplay.Objectives.Scripts
{
    public class ObjectivesManager : MonoBehaviour, ICollectionManager, MMEventListener<ObjectiveEvent>
    {
        public static HashSet<string> ActiveObjectives = new();

        public static HashSet<string> CompletedObjectives = new();

        public static HashSet<string> AllObjectives = new();

        public ObjectivesList objectivesList;

        public MMFeedbacks objectiveCompletedFeedback;


        private string _savePath;

        private void Start()
        {
            _savePath = GetSaveFilePath();

            if (!HasSavedData())
            {
                UnityEngine.Debug.Log("[ObjectivesManager] No save file found, forcing initial save...");
                ResetObjectives(); // Ensure default values are set
            }

            foreach (var objective in objectivesList.objectives) AllObjectives.Add(objective.objectiveId);

            LoadObjectives();


            StartCoroutine(InitializeAfterFrame());
        }


        private void OnEnable()
        {
            this.MMEventStartListening();
        }

        private void OnDisable()
        {
            this.MMEventStopListening();
        }

        public void OnMMEvent(ObjectiveEvent eventType)
        {
            if (eventType.type == ObjectiveEventType.ObjectiveActivated)
                // UnityEngine.Debug.Log($"Objective {eventType.objectiveId} has been activated.");
                AddActiveObjective(eventType.objectiveId);

            if (eventType.type == ObjectiveEventType.ObjectiveCompleted)
            {
                // UnityEngine.Debug.Log($"Objective {eventType.objectiveId} has been completed.");
                CompleteObjective(eventType.objectiveId);
                SaveAllObjectives();

                // Add this: Check if any objectives should now be activated
                CheckForNewObjectivesToActivate();
            }

            if (eventType.type == ObjectiveEventType.CompleteAllActiveObjectives)
                // UnityEngine.Debug.Log($"Completing all active objectives.");
                CompleteAllActiveObjectives();
        }

        private void CheckForNewObjectivesToActivate()
        {
            foreach (var objective in objectivesList.objectives)
            {
                // Skip if already active or completed
                if (IsObjectiveActive(objective.objectiveId) || IsObjectiveCompleted(objective.objectiveId))
                    continue;

                // Check if all prerequisites are met
                var allPrerequisitesMet = true;

                foreach (var prerequisite in objective.activateWhenCompleted)
                    if (!IsObjectiveCompleted(prerequisite))
                    {
                        allPrerequisitesMet = false;
                        break;
                    }

                if (allPrerequisitesMet)
                    ObjectiveEvent.Trigger(objective.objectiveId, ObjectiveEventType.ObjectiveActivated);
            }
        }

        private IEnumerator InitializeAfterFrame()
        {
            yield return null;
            var allPrerequisitesCompleted = true;

            // First, trigger events for objectives that are already active (loaded from save)
            foreach (var activeObjectiveId in ActiveObjectives)
                ObjectiveEvent.Trigger(activeObjectiveId, ObjectiveEventType.ObjectiveActivated);

            // Then check for new objectives to activate
            foreach (var objective in objectivesList.objectives)
            {
                // Skip if already active or completed
                if (IsObjectiveActive(objective.objectiveId) || IsObjectiveCompleted(objective.objectiveId))
                    continue;

                // Check ALL prerequisites are met
                var allPrerequisitesMet = true;
                foreach (var prerequisite in objective.activateWhenCompleted)
                    if (!IsObjectiveCompleted(prerequisite))
                    {
                        allPrerequisitesMet = false;
                        break;
                    }

                if (allPrerequisitesMet)
                    ObjectiveEvent.Trigger(objective.objectiveId, ObjectiveEventType.ObjectiveActivated);
            }

            SaveAllObjectives();
        }

        public static bool IsObjectiveActive(string objectiveId)
        {
            return ActiveObjectives.Contains(objectiveId);
        }

        public static bool IsObjectiveCompleted(string objectiveId)
        {
            return CompletedObjectives.Contains(objectiveId);
        }

        public void LoadObjectives()
        {
            if (_savePath == null) _savePath = GetSaveFilePath();

            if (ES3.KeyExists("CompletedObjectives", _savePath))
            {
                var completedObjectives = ES3.Load<HashSet<string>>("CompletedObjectives", _savePath);
                CompletedObjectives.Clear();

                foreach (var objective in completedObjectives) CompletedObjectives.Add(objective);
            }

            if (ES3.KeyExists("ActiveObjectives", _savePath))
            {
                var activeObjectives = ES3.Load<HashSet<string>>("ActiveObjectives", _savePath);
                ActiveObjectives.Clear();

                foreach (var objectiveId in activeObjectives) ActiveObjectives.Add(objectiveId);
            }
        }

        public static void AddActiveObjective(string objectiveId)
        {
            if (ActiveObjectives.Contains(objectiveId))
            {
                UnityEngine.Debug.LogWarning($"Objective {objectiveId} is already active.");
                return;
            }

            if (CompletedObjectives.Contains(objectiveId))
            {
                UnityEngine.Debug.LogWarning(
                    $"Objective {objectiveId} has already been completed and cannot be reactivated.");
                return;
            }


            ActiveObjectives.Add(objectiveId);
            // UnityEngine.Debug.Log($"Objective {objectiveId} added to active objectives.");
        }

        public void CompleteObjective(string objectiveId)
        {
            // if (!ActiveObjectives.Contains(objectiveId))
            // {
            //     UnityEngine.Debug.LogError($"Objective {objectiveId} is not active and cannot be completed.");
            //     return;
            // }

            if (CompletedObjectives.Contains(objectiveId))
            {
                UnityEngine.Debug.LogWarning($"Objective {objectiveId} has already been completed.");
                return;
            }


            ActiveObjectives.Remove(objectiveId);
            CompletedObjectives.Add(objectiveId);
            objectiveCompletedFeedback?.PlayFeedbacks();
        }

        public static void SaveAllObjectives()
        {
            var saveFilePath = GetSaveFilePath();

            ES3.Save("CompletedObjectives", CompletedObjectives, saveFilePath);
            ES3.Save("ActiveObjectives", ActiveObjectives, saveFilePath);
        }

        public bool HasSavedData()
        {
            return ES3.FileExists(_savePath);
        }

        public static void ResetObjectives()
        {
            CompletedObjectives = new HashSet<string>();
            ActiveObjectives = new HashSet<string>();
        }

        private static string GetSaveFilePath()
        {
            return SaveManager.SaveObjectivesFilePath;
        }

        public ObjectiveObject GetObjectiveById(string objectiveId)
        {
            foreach (var objectiveObject in objectivesList.objectives)
                if (objectiveObject.objectiveId == objectiveId)
                    return objectiveObject;

            UnityEngine.Debug.LogWarning($"Objective with ID {objectiveId} not found.");
            return null;
        }

        public static void CompleteAllActiveObjectives()
        {
            foreach (var objectiveId in ActiveObjectives)
                if (!CompletedObjectives.Contains(objectiveId))
                    ObjectiveEvent.Trigger(objectiveId, ObjectiveEventType.ObjectiveCompleted);

            SaveAllObjectives();
        }
    }
}