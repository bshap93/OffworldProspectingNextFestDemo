using System;
using Digger.Modules.Core.Sources;
using Digger.Modules.Runtime.Sources;
using Domains.Gameplay.Mining.Scripts;
using Domains.Input.Scripts;
using Domains.Player.Events;
using Domains.Scripts_that_Need_Sorting;
using Domains.UI_Global.Reticle;
using UnityEngine;
using UnityEngine.Serialization;

namespace Domains.Player.Scripts
{
    public class PlayerInteraction : MonoBehaviour
    {
        public float interactionDistance = 2f; // How far the player can interact
        public LayerMask interactableLayer; // Only detect objects in this layer
        public LayerMask terrainLayer; // Only detect objects in this layer
        public LayerMask obstacleLayer; // New: layers that block interaction (e.g., walls, rocks)


        public UnityEngine.Camera playerCamera; // Reference to the player’s camera


        [Header("Reticle")] public ReticleController reticleController;

        public LayerMask playerLayerMask;

        public bool[] diggableLayers;

        public float currentDigDepth;

        [FormerlySerializedAs("forwardTextureDetector")] [FormerlySerializedAs("ForwardTextureDetector")]
        public TerrainLayerDetector forwardTerrainLayerDetector;

        private readonly float _positionEventInterval = 0.2f; // Trigger every 0.2 seconds (5 times per second)

        private DiggerMasterRuntime _diggerMasterRuntime;
        private bool _interactablePrompt;

        private bool _mineablePrompt;

        private float _positionEventTimer;

        // private QuestJournal questJournal;

        private void Start()
        {
            FindFirstObjectByType<DiggerMaster>();
            _diggerMasterRuntime = FindFirstObjectByType<DiggerMasterRuntime>();
            GetComponent<RuntimeDig>();

            // Find the TextureDetector in the scene

            if (forwardTerrainLayerDetector == null)
                UnityEngine.Debug.LogWarning(
                    "TextureDetector not found in the scene. Cannot track texture information.");

            // questJournal = GetComponent<QuestJournal>();
            // if (questJournal == null)
            //     UnityEngine.Debug.LogWarning("QuestJournal not found in the scene. Cannot track quest information.");

            if (reticleController == null) reticleController = FindFirstObjectByType<ReticleController>();
        }

        private void Update()
        {
            PerformRaycastCheck(); // ✅ Single raycast for both interactables and diggable terrain

            // Update texture information from TextureDetector if available
            UpdateTextureInformation();
            currentDigDepth = transform.position.y;
            // PlayerPositionEvent.Trigger(PlayerPositionEventType.ReportDepth, transform.position);

            _positionEventTimer += Time.deltaTime;
            if (_positionEventTimer >= _positionEventInterval)
            {
                _positionEventTimer = 0f;
                PlayerPositionEvent.Trigger(PlayerPositionEventType.ReportDepth, transform.position);
            }

            if (CustomInputBindings.IsInteractPressed()) // Press E to interact
                PerformInteraction();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(
                playerCamera.transform.position,
                playerCamera.transform.TransformDirection(Vector3.forward) * interactionDistance);
        }

        public int GetGroundTextureIndex()
        {
            var origin = transform.position + Vector3.up * 0.1f;
            if (Physics.Raycast(origin, Vector3.down, out var hit, 2f, terrainLayer & ~playerLayerMask))
            {
                forwardTerrainLayerDetector?.UpdateFromHit(hit);
                return forwardTerrainLayerDetector?.textureIndex ?? -1;
            }

            return -1;
        }


        // New method to update texture information
        private void UpdateTextureInformation()
        {
            if (forwardTerrainLayerDetector != null && !string.IsNullOrEmpty(forwardTerrainLayerDetector.texture))
                // Extract name and index from TextureDetector's texture string
                if (ExtractNameAndIndex(forwardTerrainLayerDetector.texture, out var name, out var index))
                {
                    // Update our tracking variables
                }
        }


        private void PerformRaycastCheck()
        {
            if (playerCamera == null)
            {
                UnityEngine.Debug.LogError("PlayerInteraction: No camera assigned!");
                return;
            }

            var rayOrigin = playerCamera.transform.position;
            var rayDirection = playerCamera.transform.forward;

            var terrMask = terrainLayer & ~playerLayerMask;
            var interactMask = interactableLayer & ~playerLayerMask;

            // Combined raycast check
            RaycastHit terrainHit;
            var terrainBlocking =
                Physics.Raycast(rayOrigin, rayDirection, out terrainHit, interactionDistance, terrMask);

            if (terrainBlocking) forwardTerrainLayerDetector?.UpdateFromHit(terrainHit);

            RaycastHit interactableHit;
            var hitInteractable = Physics.Raycast(rayOrigin, rayDirection, out interactableHit, interactionDistance,
                interactMask);

            // Determine the hit to process
            RaycastHit? actualHit = null;
            var isTerrainBlocking = false;

            if (terrainBlocking && hitInteractable)
            {
                if (terrainHit.distance < interactableHit.distance)
                    isTerrainBlocking = true;
                else
                    actualHit = interactableHit;
            }
            else if (hitInteractable)
            {
                actualHit = interactableHit;
            }

            // Update reticle through controller
            reticleController.UpdateReticle(actualHit, isTerrainBlocking);

            // Show/hide prompts as needed (existing logic)
            if (actualHit.HasValue)
            {
                var interactable = actualHit.Value.collider.GetComponent<IInteractable>();
                var button = actualHit.Value.collider.GetComponent<ButtonActivated>();
                var mineable = actualHit.Value.collider.GetComponent<IMinable>();

                if (interactable != null)
                {
                    interactable.ShowInteractablePrompt();
                    _interactablePrompt = true;
                    if (button != null) button.ShowInteractablePrompt();
                }
                else if (mineable != null)
                {
                    mineable.ShowMineablePrompt();
                    _mineablePrompt = true;
                }
            }
            else
            {
                if (_interactablePrompt)
                    _interactablePrompt = false;
                HideAllPrompts();
            }
        }

        private void HideAllPrompts()
        {
            foreach (var button in FindObjectsByType<ButtonActivated>(FindObjectsSortMode.None))
                button.HideInteractablePrompt();

            foreach (var buttonWithAction in FindObjectsByType<ButtonActivatedWithAction>(FindObjectsSortMode.None))
                buttonWithAction.HideInteractablePrompt();

            foreach (var healthButton in FindObjectsByType<HealthButtonActivatedWithAction>(FindObjectsSortMode.None))
                healthButton.HideInteractablePrompt();

            foreach (var infoPanel in FindObjectsByType<InfoPanelActivator>(FindObjectsSortMode.None))
                infoPanel.HideInfoPanel();
        }

        /// <summary>
        ///     Extracts name and index from a string in the format "name: Topsoil | index: 0"
        /// </summary>
        /// <param name="input">The formatted string to parse</param>
        /// <param name="name">Output parameter that will contain the extracted name</param>
        /// <param name="index">Output parameter that will contain the extracted index</param>
        /// <returns>True if parsing was successful, false otherwise</returns>
        public bool ExtractNameAndIndex(string input, out string name, out int index)
        {
            // Initialize output parameters with default values
            name = string.Empty;
            index = -1;

            // Check if input is null or empty
            if (string.IsNullOrEmpty(input))
                return false;

            try
            {
                // Split the input by the separator '|'
                var parts = input.Split('|');

                if (parts.Length < 2)
                    return false;

                // Extract name part and trim whitespace
                var namePart = parts[0].Trim();

                // Extract index part and trim whitespace
                var indexPart = parts[1].Trim();

                // Check if the parts start with expected prefixes
                if (!namePart.StartsWith("name:") || !indexPart.StartsWith("index:"))
                    return false;

                // Extract the actual name (remove "name: " prefix and trim)
                name = namePart.Substring(5).Trim();

                // Extract the actual index (remove "index: " prefix and trim)
                var indexValue = indexPart.Substring(6).Trim();

                // Parse index to integer
                if (!int.TryParse(indexValue, out index))
                    return false;

                return true;
            }
            catch (Exception)
            {
                // Return false in case of any exception
                return false;
            }
        }


        private void PerformInteraction()
        {
            if (playerCamera == null)
            {
                UnityEngine.Debug.LogError("PlayerInteraction: No camera assigned!");
                return;
            }

            var rayOrigin = playerCamera.transform.position;
            var rayDirection = playerCamera.transform.forward;

            var interactMask = interactableLayer & ~playerLayerMask;
            var terrMask = terrainLayer & ~playerLayerMask;
            var obstacleMask = obstacleLayer & ~playerLayerMask; // Add obstacle mask

            // Check for obstacles first
            RaycastHit obstacleHit;
            var obstacleBlocking = Physics.Raycast(
                rayOrigin, rayDirection, out obstacleHit, interactionDistance, obstacleMask);

            // Check if terrain is blocking
            RaycastHit terrainHit;
            var terrainBlocking = Physics.Raycast(
                rayOrigin, rayDirection, out terrainHit, interactionDistance, terrMask);

            // Check for interactables
            RaycastHit interactableHit;
            var hitInteractable = Physics.Raycast(
                rayOrigin, rayDirection, out interactableHit, interactionDistance, interactMask);

            // Only interact if:
            // 1. We hit an interactable AND
            // 2. No obstacles are blocking AND
            // 3. Either there's no terrain blocking OR the interactable is closer than the terrain
            if (hitInteractable &&
                (!obstacleBlocking || interactableHit.distance < obstacleHit.distance) &&
                (!terrainBlocking || interactableHit.distance < terrainHit.distance))
            {
                var interactable = interactableHit.collider.GetComponent<IInteractable>();
                if (interactable != null)
                    interactable.Interact();
            }
        }

        // private void PerformInteraction()
        // {
        //     if (playerCamera == null)
        //     {
        //         UnityEngine.Debug.LogError("PlayerInteraction: No camera assigned!");
        //         return;
        //     }
        //
        //     var rayOrigin = playerCamera.transform.position;
        //     var rayDirection = playerCamera.transform.forward;
        //
        //     var interactMask = interactableLayer & ~playerLayerMask;
        //     var terrMask = terrainLayer & ~playerLayerMask;
        //
        //     // First check if terrain is blocking
        //     RaycastHit terrainHit;
        //     var terrainBlocking = Physics.Raycast(
        //         rayOrigin, rayDirection, out terrainHit, interactionDistance, terrMask);
        //
        //     // Then check for interactables
        //     RaycastHit interactableHit;
        //     var hitInteractable = Physics.Raycast(
        //         rayOrigin, rayDirection, out interactableHit, interactionDistance, interactMask);
        //
        //     // Only interact if:
        //     // 1. We hit an interactable AND
        //     // 2. Either there's no terrain blocking OR the interactable is closer than the terrain
        //     if (hitInteractable && (!terrainBlocking || interactableHit.distance < terrainHit.distance))
        //     {
        //         var interactable = interactableHit.collider.GetComponent<IInteractable>();
        //         if (interactable != null) interactable.Interact();
        //     }
        // }
    }
}