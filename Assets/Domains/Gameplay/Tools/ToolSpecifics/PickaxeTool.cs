using System;
using System.Collections;
using Digger.Modules.Core.Sources;
using Digger.Modules.Runtime.Sources;
using Domains.Debug;
using Domains.Gameplay.Managers;
using Domains.Gameplay.Mining.Scripts;
using Domains.Player.Events;
using Domains.Player.Scripts;
using Domains.Scripts_that_Need_Sorting;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Serialization;

namespace Domains.Gameplay.Tools.ToolSpecifics
{
    public class PickaxeTool : BaseDiggerUsingTool, MMEventListener<UpgradeEvent>
    {
        [Header("Stat Settings")] public int hardnessCanBreak;

        [SerializeField] private float delayBeforeDigging = 0.1f;

        [SerializeField] private MMFeedbacks firstHitFeedbacks;
        [SerializeField] private MMFeedbacks secondHitFeedbacks;
        [Header("Debris Effects")] public GameObject debrisEffectFirstHitPrefab;

        [SerializeField] private float firstHitEffectOpacity;
        [SerializeField] private float firstHitEffectRadius;

        public GameObject debrisEffectSecondHitPrefab;

        [FormerlySerializedAs("diggingFeedbacks")] [SerializeField]
        private MMFeedbacks pickaxeBehavior;

        public Material currentMaterial;

        [SerializeField] private ProgressBarBlue cooldownProgressBar;
        private readonly float defaultEffectOpacity = 10f;


        // Backup default values to restore if needed
        private readonly float defaultEffectRadius = 0.8f;
        private readonly float defaultFirstHitOpacity = 1f;
        private readonly float defaultFirstHitRadius = 0.1f;
        private readonly float defaultStalagmiteHeight = 100f;

        [Header("Hit Number Logic")] private readonly float hitThresholdDistance = 0.5f; // adjust as needed

        private new readonly float maxEffectOpacity = 100f;

        // Maximum safe values
        private new readonly float maxEffectRadius = 2f;
        private readonly float maxStalagmiteHeight = 200f;

        // Track active coroutine to prevent multiple digs happening at once
        private Coroutine activeDigCoroutine;

        // Flag to force validation in Update
        private bool forceValidation = true;

        // Flag to prevent race conditions
        private bool isDigging;

        private Vector3 lastHitPosition;

        private GameObject lastValidDebrisPrefab;

        // Logger reference
        private DiggerDebugLogger logger;
        private MeshRenderer meshRenderer;
        private TerrainController terrainController;
        private int terrainHitCount;
        private float validationTimer;

        private void Awake()
        {
            // Find or create logger
            logger = DiggerDebugLogger.Instance;
            if (logger == null)
            {
                var loggerObject = new GameObject("DiggerDebugLogger");
                logger = loggerObject.AddComponent<DiggerDebugLogger>();
            }

            logger.LogMessage("PickaxeTool.Awake - initializing");

            digger = FindFirstObjectByType<DiggerMasterRuntime>();
            playerInteraction = FindFirstObjectByType<PlayerInteraction>();
            meshRenderer = GetComponent<MeshRenderer>();

            terrainController = FindFirstObjectByType<TerrainController>();
            if (terrainController == null) logger.LogError("TerrainController not found in scene");

            // Initialize values to prevent NaN
            firstHitEffectOpacity = ClampFloat(firstHitEffectOpacity, 1f, 50f);
            firstHitEffectRadius = ClampFloat(firstHitEffectRadius, 0.1f, 5f);
            effectOpacity = ClampFloat(effectOpacity, 1f, 150f);
            effectRadius = ClampFloat(effectRadius, 0.1f, 5f);

            logger.LogMessage(
                $"PickaxeTool initialized with parameters: firstHitOpacity={firstHitEffectOpacity}, firstHitRadius={firstHitEffectRadius}, mainOpacity={effectOpacity}, mainRadius={effectRadius}");
        }

        private void Start()
        {
            // Initialize cooldown bar
            HideCooldownBar();
        }

        private void Update()
        {
            // Validate parameters periodically or when forced
            validationTimer += Time.deltaTime;
            if (validationTimer > 1f || forceValidation)
            {
                ValidateAllParameters();
                validationTimer = 0f;
                forceValidation = false;
            }
        }

        private void OnEnable()
        {
            this.MMEventStartListening();

            // Force validation whenever tool is enabled
            forceValidation = true;
            ValidateAllParameters();

            // Reapply current material when enabled
            if (currentMaterial != null) SetCurrentMaterial(currentMaterial);
            // if (debugLogging)
            //     UnityEngine.Debug.Log($"{GetType().Name} enabled - reapplied material: {currentMaterial.name}");
            // Reset state when tool is enabled
            isDigging = false;
            terrainHitCount = 0;
            if (activeDigCoroutine != null)
            {
                StopCoroutine(activeDigCoroutine);
                activeDigCoroutine = null;
            }

            // if (debugLogging) UnityEngine.Debug.Log("PickaxeTool enabled and reset");
        }

        private void OnDisable()
        {
            this.MMEventStopListening();

            // Cancel any ongoing digging when tool is disabled
            if (activeDigCoroutine != null)
            {
                StopCoroutine(activeDigCoroutine);
                activeDigCoroutine = null;
            }

            isDigging = false;

            logger?.LogMessage("PickaxeTool disabled and reset");
        }

        public void OnMMEvent(UpgradeEvent eventType)
        {
            if (eventType.EventType == UpgradeEventType.PickaxeMiningSizeSet)
            {
                if (debugLogging)
                    UnityEngine.Debug.Log(
                        $"Received PickaxeMiningSizeSet event: {eventType.EffectValue}, {eventType.EffectValue2}");

                // Apply the upgrade but then force validation
                SetDiggerUsingToolEffectSize(eventType.EffectValue, eventType.EffectValue2);
                forceValidation = true;
                ValidateAllParameters();
            }
        }

        // Add this method to PickaxeTool.cs to ensure all parameters are valid before every dig operation
        private bool EnsureDigParametersValid()
        {
            // Check core references
            if (digger == null || playerInteraction == null || mainCamera == null)
            {
                UnityEngine.Debug.LogError("Missing core references for digging operation");
                return false;
            }

            // Validate effect parameters
            if (!IsValidFloat(effectRadius) || !IsValidFloat(effectOpacity) || !IsValidFloat(stalagmiteHeight))
            {
                UnityEngine.Debug.LogError(
                    $"Invalid effect parameters: radius={effectRadius}, opacity={effectOpacity}, height={stalagmiteHeight}");

                // Auto-fix the parameters
                effectRadius = ClampFloat(effectRadius, minEffectRadius, maxEffectRadius);
                effectOpacity = ClampFloat(effectOpacity, minEffectOpacity, maxEffectOpacity);
                stalagmiteHeight = ClampFloat(stalagmiteHeight, 1f, 200f);

                return effectRadius > 0 && effectOpacity > 0 && stalagmiteHeight > 0;
            }

            return true;
        }

        private void ValidateAllParameters()
        {
            var hadInvalidValue = false;

            // Check main effect parameters
            if (!IsValidFloat(effectRadius))
            {
                UnityEngine.Debug.LogError($"Invalid effect radius detected: {effectRadius}. Resetting to default.");
                effectRadius = defaultEffectRadius;
                hadInvalidValue = true;
            }

            if (!IsValidFloat(effectOpacity))
            {
                UnityEngine.Debug.LogError($"Invalid effect opacity detected: {effectOpacity}. Resetting to default.");
                effectOpacity = defaultEffectOpacity;
                hadInvalidValue = true;
            }

            if (!IsValidFloat(firstHitEffectRadius))
            {
                UnityEngine.Debug.LogError(
                    $"Invalid first hit radius detected: {firstHitEffectRadius}. Resetting to default.");
                firstHitEffectRadius = defaultFirstHitRadius;
                hadInvalidValue = true;
            }

            if (!IsValidFloat(firstHitEffectOpacity))
            {
                UnityEngine.Debug.LogError(
                    $"Invalid first hit opacity detected: {firstHitEffectOpacity}. Resetting to default.");
                firstHitEffectOpacity = defaultFirstHitOpacity;
                hadInvalidValue = true;
            }

            if (!IsValidFloat(stalagmiteHeight))
            {
                UnityEngine.Debug.LogError(
                    $"Invalid stalagmite height detected: {stalagmiteHeight}. Resetting to default.");
                stalagmiteHeight = defaultStalagmiteHeight;
                hadInvalidValue = true;
            }

            // Clamp to safe ranges regardless
            effectRadius = Mathf.Clamp(effectRadius, 0.1f, maxEffectRadius);
            effectOpacity = Mathf.Clamp(effectOpacity, 1f, maxEffectOpacity);
            firstHitEffectRadius = Mathf.Clamp(firstHitEffectRadius, 0.05f, maxEffectRadius / 2f);
            firstHitEffectOpacity = Mathf.Clamp(firstHitEffectOpacity, 0.5f, maxEffectOpacity / 2f);
            stalagmiteHeight = Mathf.Clamp(stalagmiteHeight, 1f, maxStalagmiteHeight);

            if (hadInvalidValue)
                UnityEngine.Debug.Log($"After validation: radius={effectRadius}, opacity={effectOpacity}, " +
                                      $"firstHitRadius={firstHitEffectRadius}, firstHitOpacity={firstHitEffectOpacity}, " +
                                      $"height={stalagmiteHeight}");
        }

        public void SetCurrentMaterial(Material material)
        {
            currentMaterial = material;
            if (meshRenderer != null)
                meshRenderer.material = currentMaterial;
        }

        public override void UseTool(RaycastHit hit)
        {
            // Validate hit before processing
            if (!IsValidVector3(hit.point) || !IsValidVector3(hit.normal))
            {
                logger?.LogError($"Invalid hit data: point={hit.point}, normal={hit.normal}");
                return;
            }

            logger?.LogMessage("UseTool called with valid hit");
            logger?.LogVector3("Hit point", hit.point);

            lastHit = hit;
            PerformToolAction();
        }

        public override void PerformToolAction()
        {
            // Prevent multiple simultaneous digging operations
            if (isDigging)
            {
                logger?.LogMessage("Already digging, skipping action");
                return;
            }

            try
            {
                // Check cooldown
                if (Time.time < lastDigTime + miningCooldown)
                {
                    logger?.LogMessage("Tool on cooldown, skipping action");
                    return;
                }

                // Update last dig time
                lastDigTime = Time.time;

                // Validate core references
                if (playerInteraction == null || digger == null)
                {
                    logger?.LogError("Missing references: playerInteraction or digger is null");
                    return;
                }

                // Validate camera
                if (mainCamera == null)
                {
                    logger?.LogError("Main camera reference is null");
                    return;
                }

                // Sanitize camera direction
                var cameraPosition = SanitizeVector3(mainCamera.transform.position);
                var cameraDirection = SanitizeVector3(mainCamera.transform.forward);

                logger?.LogVector3("Camera position", cameraPosition);
                logger?.LogVector3("Camera direction", cameraDirection);

                if (!IsValidVector3(cameraDirection))
                {
                    logger?.LogError("Invalid camera direction after sanitization");
                    return;
                }

                // Confirm we have a valid ray hit
                var notPlayerMask = ~playerInteraction.playerLayerMask;
                RaycastHit hit;

                logger?.LogMessage(
                    $"Performing raycast: origin={cameraPosition}, direction={cameraDirection}, range={diggerUsingRange}");

                if (!Physics.Raycast(cameraPosition, cameraDirection, out hit, diggerUsingRange, notPlayerMask))
                {
                    logger?.LogMessage("No valid raycast hit found");
                    return;
                }

                // Cache hit for external access
                lastHit = hit;

                logger?.LogVector3("Hit position", hit.point);
                logger?.LogVector3("Hit normal", hit.normal);

                // Validate hit has a collider
                if (hit.collider == null)
                {
                    logger?.LogError("Hit collider is null");
                    return;
                }

                // Get texture index from hit - use a safe default if it fails
                var detectedTextureIndex = 0;
                try
                {
                    detectedTextureIndex = terrainLayerDetector.GetTextureIndex(lastHit, out _);
                    logger?.LogMessage($"Detected texture index: {detectedTextureIndex}");
                }
                catch (Exception ex)
                {
                    logger?.LogError("Failed to get texture index", ex);
                    // Continue with the default value
                }

                // Check if we're hitting a minable object
                var isMinableObject = CanInteractWithObject(lastHit.collider.gameObject);

                // Check if we're hitting valid terrain
                var isTerrain = detectedTextureIndex >= 0;
                var isValidTerrain = CanInteractWithTextureIndex(detectedTextureIndex);

                logger?.LogMessage(
                    $"Hit info - isMinableObject: {isMinableObject}, isTerrain: {isTerrain}, isValidTerrain: {isValidTerrain}");

                // Exit early if we're not hitting anything valid
                if (!isMinableObject && (!isTerrain || !isValidTerrain))
                {
                    logger?.LogMessage("Not hitting valid target, exiting");
                    return;
                }

                // Handle object interaction first (ore, rocks, etc.)
                if (isMinableObject) HandleObjectInteraction(hit);

                // Exit if not a valid terrain texture
                if (!isValidTerrain)
                {
                    logger?.LogMessage("Not valid terrain texture, exiting");
                    return;
                }

                // Get the actual texture index based on depth and overrides
                int textureIndex;
                try
                {
                    textureIndex = GetTerrainLayerBasedOnDepthAndOverrides(detectedTextureIndex, lastHit.point.y);
                    logger?.LogMessage($"Final texture index after processing: {textureIndex}");
                }
                catch (Exception ex)
                {
                    logger?.LogError("Failed to process terrain layer", ex);
                    textureIndex = 0; // Use a safe default
                }

                CooldownCoroutine = StartCoroutine(cooldownProgressBar.ShowCooldownBarCoroutine(miningCooldown));
                // Handle terrain digging
                HandleTerrainDigging(hit, textureIndex);
            }
            catch (Exception ex)
            {
                logger?.LogError("Pickaxe error in PerformToolAction", ex);
                // Reset state on error
                isDigging = false;
                if (activeDigCoroutine != null)
                {
                    StopCoroutine(activeDigCoroutine);
                    activeDigCoroutine = null;
                }
            }
        }

        private void HandleObjectInteraction(RaycastHit hit)
        {
            try
            {
                logger?.LogMessage("HandleObjectInteraction started");

                // Call IInteractable if implemented
                var interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    interactable.Interact();
                    logger?.LogMessage("Called Interact() on interactable object");
                }

                // Handle minable objects
                var minable = hit.collider.GetComponent<IMinable>();
                if (minable != null)
                {
                    var objectHardness = minable.GetCurrentMinableHardness();
                    logger?.LogMessage(
                        $"Mining object with hardness {objectHardness} vs tool hardness {hardnessCanBreak}");


                    if (objectHardness <= hardnessCanBreak)
                    {
                        minable.MinableMineHit();
                        moveToolDespiteFailHitFeedbacks?.PlayFeedbacks();
                        logger?.LogMessage("Successfully mined object");
                    }
                    else
                    {
                        minable.MinableFailHit(hit.point);
                        moveToolDespiteFailHitFeedbacks?.PlayFeedbacks();
                        logger?.LogMessage("Failed to mine object - hardness too high");
                    }

                    CooldownCoroutine = StartCoroutine(cooldownProgressBar.ShowCooldownBarCoroutine(miningCooldown));
                }
            }
            catch (Exception ex)
            {
                logger?.LogError("Error in HandleObjectInteraction", ex);
            }
        }


// Replace the current HandleTerrainDigging method in PickaxeTool.cs with this improved version
        private void HandleTerrainDigging(RaycastHit hit, int textureIndex)
        {
            try
            {
                // Force validation of all parameters
                ValidateAllParameters();

                // Additional safety check
                if (!EnsureDigParametersValid())
                {
                    UnityEngine.Debug.LogError("Dig parameters invalid and could not be repaired");
                    return;
                }

                // Validate hit point and normal
                if (!IsValidVector3(hit.point) || !IsValidVector3(hit.normal))
                {
                    UnityEngine.Debug.LogError($"Invalid hit data: point={hit.point}, normal={hit.normal}");
                    return;
                }

                // Distance check for hit count
// reset and clamp FIRST, so radius is always > 0
                firstHitEffectRadius = ClampFloat(firstHitEffectRadius, 0.1f, 2f);
                firstHitEffectOpacity = ClampFloat(firstHitEffectOpacity, 1f, 50f);

                if (terrainHitCount > 0 &&
                    Vector3.Distance(hit.point, lastHitPosition) > hitThresholdDistance)
                    terrainHitCount = 0;

                // Update hit tracking
                lastHitPosition = hit.point;
                terrainHitCount++;

                // Check if already digging - abort if so
                if (isDigging)
                {
                    UnityEngine.Debug.Log("Already digging, ignoring new dig request");
                    return;
                }

                // First hit - smaller impression
                if (terrainHitCount == 1)
                {
                    // Calculate dig position with safety checks
                    var cameraForward = SanitizeVector3(mainCamera.transform.forward);
                    var digPositionFirst = hit.point + cameraForward * 0.3f;

                    // Make safe copies of parameters that can't be modified during the coroutine
                    var safeRadius = ClampFloat(firstHitEffectRadius, 0.1f, 2f);
                    var safeOpacity = ClampFloat(firstHitEffectOpacity, 1f, 50f);

                    // Set digging flag
                    isDigging = true;

                    // Cancel any existing coroutine
                    if (activeDigCoroutine != null) StopCoroutine(activeDigCoroutine);

                    // Start new coroutine
                    activeDigCoroutine = StartCoroutine(Dig(
                        digPositionFirst,
                        textureIndex,
                        safeOpacity,
                        safeRadius));

                    // Effects
                    TriggerDebrisEffect(debrisEffectFirstHitPrefab, hit);
                    firstHitFeedbacks?.PlayFeedbacks(hit.point);
                    return;
                }

                // Second hit breaks the terrain
                if (terrainHitCount >= 2)
                {
                    // Reset hit count
                    terrainHitCount = 0;

                    // Calculate dig position with safety
                    var cameraForward = SanitizeVector3(mainCamera.transform.forward);
                    var digPosition = hit.point + cameraForward * 0.3f;

                    // Safe copies of parameters
                    var safeRadius = ClampFloat(effectRadius, 0.1f, 5f);
                    var safeOpacity = ClampFloat(effectOpacity, 1f, 200f);

                    var debrisEffectPrefab = terrainController.GetTerrainPrefab(textureIndex);

                    if (debrisEffectPrefab != null)
                        debrisEffectSecondHitPrefab = debrisEffectPrefab;
                    else
                        UnityEngine.Debug.LogWarning($"No debris effect prefab found for texture index {textureIndex}");

                    // Set digging flag
                    isDigging = true;

                    // Cancel any existing coroutine
                    if (activeDigCoroutine != null) StopCoroutine(activeDigCoroutine);


                    // Start new coroutine
                    activeDigCoroutine = StartCoroutine(Dig(
                        digPosition,
                        textureIndex,
                        safeOpacity,
                        safeRadius));

                    // Effects
                    TriggerDebrisEffect(debrisEffectSecondHitPrefab, hit);
                    secondHitFeedbacks?.PlayFeedbacks(hit.point);
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"Error in HandleTerrainDigging: {ex.Message}\n{ex.StackTrace}");
                // Reset state on error
                isDigging = false;
                terrainHitCount = 0;
                if (activeDigCoroutine != null)
                {
                    StopCoroutine(activeDigCoroutine);
                    activeDigCoroutine = null;
                }
            }
        }


// Modify your Dig coroutine to validate parameters before passing to SafeModify
        private IEnumerator Dig(Vector3 digPosition, int textureIndex,
            float effectOpacityLoc, float effectRadiusLoc, BrushType brushLoc = BrushType.Sphere)
        {
            if (debugLogging)
                UnityEngine.Debug.Log(
                    $"Starting dig at position {digPosition}, texture {textureIndex}, opacity {effectOpacityLoc}, radius {effectRadiusLoc}");

            // Create safe copies that can't be modified elsewhere
            var radius = effectRadiusLoc;
            var opacity = effectOpacityLoc;
            var height = stalagmiteHeight;

            // Make sure none of these values can be NaN or infinity
            if (!IsValidFloat(radius)) radius = defaultEffectRadius;
            if (!IsValidFloat(opacity)) opacity = defaultEffectOpacity;
            if (!IsValidFloat(height)) height = defaultStalagmiteHeight;

// 1. make safe snapshots
            var safeRadius = Mathf.Clamp(effectRadiusLoc, 0.1f, maxEffectRadius);
            var safeOpacity = Mathf.Clamp(effectOpacityLoc, 1f, 200f);
            var safeHeight = Mathf.Clamp(stalagmiteHeight, 1f, maxStalagmiteHeight);

            // Wait for delay - outside the try-catch
            yield return new WaitForSeconds(delayBeforeDigging);

            try
            {
                // Check if digger reference is still valid
                if (digger == null)
                {
                    UnityEngine.Debug.LogError("Digger reference lost");
                    isDigging = false;
                    yield break;
                }

                // Double-check all values for safety
                if (!IsValidVector3(digPosition))
                {
                    UnityEngine.Debug.LogError($"Invalid dig position before modify: {digPosition}");
                    isDigging = false;
                    yield break;
                }

                // One final check before digging
                if (!IsValidFloat(radius) || !IsValidFloat(opacity) || !IsValidFloat(height))
                {
                    UnityEngine.Debug.LogError(
                        $"Invalid parameters detected right before digging: radius={radius}, opacity={opacity}, height={height}");
                    isDigging = false;
                    yield break;
                }

                if (debugLogging)
                    UnityEngine.Debug.Log($"Final dig parameters: radius={radius}, opacity={opacity}, height={height}");

                // Use the safe modify method to ensure no NaN values reach the Digger system
// 3. use the snapshots
                var didModify = SafeModify(digPosition, brushLoc, Action,
                    textureIndex, safeOpacity, safeRadius, safeHeight);
                if (!didModify)
                    yield break;


// after SafeModify you KNOW parameters are finite.
                // UnityEngine.Debug.Log($"DIG {Time.frameCount}: p={digPosition} r={radius} o={opacity}");
                SimpleGravityIntegration.OnDigPerformed(digPosition, effectRadius * 2f);
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"Dig coroutine error: {ex.Message}\n{ex.StackTrace}");
            }

            // Wait a frame to ensure digging operation had time to process - outside the try-catch
            yield return null;

            // Finally block equivalent - always executes
            isDigging = false;
            activeDigCoroutine = null;
            if (debugLogging) UnityEngine.Debug.Log("Dig operation complete, reset state");
        }
    }
}