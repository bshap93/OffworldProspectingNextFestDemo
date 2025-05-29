using System;
using Domains.Effects.Scripts;
using Domains.Gameplay.Mining.Scripts;
using Domains.Items.Scripts;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

namespace Domains.Debug
{
    public class ManualItemPickerIDAssigner : Editor
    {
        [MenuItem("Debug/Assign Unique IDs to OreNodes")]
        private static void AssignUniqueIDsOreNode()
        {
            // Find all ManualItemPicker components in the current scene
            var allOreNodes = FindObjectsByType<OreNode>(FindObjectsSortMode.None);

            // Check if any were found
            if (allOreNodes.Length == 0)
            {
                UnityEngine.Debug.LogWarning("No ManualItemPicker components found in the scene.");
                return;
            }

            // Iterate through each ManualItemPicker and assign a unique ID
            foreach (var oreNode in allOreNodes)
                if (oreNode != null)
                {
                    // Generate a unique ID using GUID
                    oreNode.UniqueID = Guid.NewGuid().ToString();
                    EditorUtility.SetDirty(oreNode); // Mark the object as dirty for saving
                }

            // Save the scene to persist changes
            AssetDatabase.SaveAssets();

            UnityEngine.Debug.Log($"Assigned unique IDs to {allOreNodes.Length} ManualItemPicker components.");
        }

        [MenuItem("Debug/Assign Unique IDs to HighlightableItems")]
        private static void AssignUniqueHighlightTargetIDs()
        {
            var allHighlightableItems = FindObjectsByType<HighlightEffectController>(FindObjectsSortMode.None);

            if (allHighlightableItems.Length == 0)
            {
                UnityEngine.Debug.LogWarning("No HighlightableItems found in the scene.");
                return;
            }

            foreach (var highlightableItem in allHighlightableItems)
                if (highlightableItem != null)
                {
                    // Generate a unique ID using GUID
                    highlightableItem.targetID = Guid.NewGuid().ToString();
                    EditorUtility.SetDirty(highlightableItem); // Mark the object as dirty for saving
                }

            // Save the scene to persist changes
            AssetDatabase.SaveAssets();

            UnityEngine.Debug.Log($"Assigned unique IDs to {allHighlightableItems.Length} HighlightableItems.");
        }

        [MenuItem("Debug/Assign Unique IDs to ItemPickers")]
        private static void AssignUniqueIDs()
        {
            // Find all ManualItemPicker components in the current scene
            var allItemPickers = FindObjectsByType<ItemPicker>(FindObjectsSortMode.None);

            // Check if any were found
            if (allItemPickers.Length == 0)
            {
                UnityEngine.Debug.LogWarning("No ManualItemPicker components found in the scene.");
                return;
            }

            // Iterate through each ManualItemPicker and assign a unique ID
            foreach (var picker in allItemPickers)
                if (picker != null)
                {
                    // Generate a unique ID using GUID
                    picker.uniqueID = Guid.NewGuid().ToString();
                    EditorUtility.SetDirty(picker); // Mark the object as dirty for saving
                }

            // Save the scene to persist changes
            AssetDatabase.SaveAssets();

            UnityEngine.Debug.Log($"Assigned unique IDs to {allItemPickers.Length} ManualItemPicker components.");
        }

        [MenuItem("Debug/Assign Unique IDs to InteractableObjectives")]
        private static void AssignUniqueIDsInteractableObjectives()
        {
            // Find all InteractableObjective components in the current scene
            var allInteractableObjectives = FindObjectsByType<InteractableObjective>(FindObjectsSortMode.None);

            // Check if any were found
            if (allInteractableObjectives.Length == 0)
            {
                UnityEngine.Debug.LogWarning("No InteractableObjective components found in the scene.");
                return;
            }

            // Iterate through each InteractableObjective and assign a unique ID
            foreach (var interactableObjective in allInteractableObjectives)
                if (interactableObjective != null)
                {
                    // Generate a unique ID using GUID
                    interactableObjective.uniqueID = Guid.NewGuid().ToString();
                    EditorUtility.SetDirty(interactableObjective); // Mark the object as dirty for saving
                }

            // Save the scene to persist changes
            AssetDatabase.SaveAssets();

            UnityEngine.Debug.Log($"Assigned unique IDs to {allInteractableObjectives.Length} InteractableObjectives.");
        }
    }
}
#endif