using Domains.Player.Events;
using Domains.Player.Scripts;
using MoreMountains.Tools;
using TMPro;
using UnityEngine;
using PointedObjectInfo = Domains.Player.Events.PointedObjectInfo;

namespace Domains.UI_Global.Scripts
{
    public class PointedObjectDisplay : MonoBehaviour, MMEventListener<PointedObjectEvent>
    {
        [Header("References")] public PlayerInteraction playerInteraction;
        public TextMeshProUGUI objectNameText;
        public TextMeshProUGUI objectTypeText;

        [Header("Settings")] public Color terrainColor = Color.green;
        public Color interactableColor = Color.yellow;
        public Color defaultColor = Color.white;

        private void Start()
        {
            if (playerInteraction == null)
            {
                playerInteraction = FindFirstObjectByType<PlayerInteraction>();
                if (playerInteraction == null)
                {
                    UnityEngine.Debug.LogError("PointedObjectDisplay: No PlayerInteraction found in scene!");
                    enabled = false;
                    return;
                }
            }

            // Initialize texts
            if (objectNameText != null)
                objectNameText.text = "";

            if (objectTypeText != null)
                objectTypeText.text = "";
        }

        private void OnEnable()
        {
            this.MMEventStartListening();
        }

        private void OnDisable()
        {
            this.MMEventStopListening();
        }


        private void OnPointedObjectChanged(PointedObjectInfo objectInfo)
        {
            if (objectNameText != null)
            {
                objectNameText.text = objectInfo.name;

                // Set color based on type
                if (objectInfo.type == PointedObjectType.Terrain)
                    objectNameText.color = terrainColor;
                else if (objectInfo.isInteractable)
                    objectNameText.color = interactableColor;
                else
                    objectNameText.color = defaultColor;
            }

            if (objectTypeText != null)
            {
                var typeText = GetPointedTypeName(objectInfo.type);

                // Add texture index for terrain
                if (objectInfo.type == PointedObjectType.Terrain && objectInfo.textureIndex >= 0)
                    typeText = $"{GetTextureName(objectInfo.textureIndex)})";

                // Add interactable indicator
                if (objectInfo.isInteractable)
                    typeText += " [E to interact]";

                objectTypeText.text = typeText;
            }
        }

        public void OnMMEvent(PointedObjectEvent eventType)
        {
            OnPointedObjectChanged(eventType.PointedObjectInfo);
        }

        private string GetTextureName(int textureIndex)
        {
            switch (textureIndex)
            {
                case 0:
                    return "Surface";
                case 1:
                    return "Weathered Dirt";
                case 2:
                    return "Dirt";
                case 3:
                    return "Sediment";

                default:
                    return "Unknown";
            }
        }

        private string GetPointedTypeName(PointedObjectType type)
        {
            switch (type)
            {
                case PointedObjectType.Terrain:
                    return "Terrain";
                case PointedObjectType.Interactable:
                    return "Interactable";
                default:
                    return "Unknown";
            }
        }
    }
}