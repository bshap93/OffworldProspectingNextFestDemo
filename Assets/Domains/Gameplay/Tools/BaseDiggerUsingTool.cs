using System;
using Digger.Modules.Core.Sources;
using Digger.Modules.Runtime.Sources;
using Domains.Gameplay.Objects.Powerups;
using Domains.Player.Scripts;
using Domains.Scripts_that_Need_Sorting;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Serialization;

namespace Domains.Gameplay.Tools
{
    public abstract class BaseDiggerUsingTool : MonoBehaviour, IToolAction, MMEventListener<PowerUpEvent>
    {
        [Header("Dig Settings")] public float diggerUsingRange = 5f;

        [Header("Effect Settings")] public float minEffectRadius = 0.3f;

        public float maxEffectRadius = 1.2f;
        public float minEffectOpacity = 5f;
        public float maxEffectOpacity = 150f;
        [SerializeField] protected float miningCooldown = 1f; // seconds between digs


        [Header("Tool Settings")]
        [Tooltip("Feedbacks to play when the tool is used")]
        [FormerlySerializedAs("moveShovelDespiteFailHitFeedbacks")]
        [SerializeField]
        protected MMFeedbacks moveToolDespiteFailHitFeedbacks;

        public float effectRadius = 1f;
        public float effectOpacity = 10f;
        public float stalagmiteHeight = 100f;

        public BrushType brush = BrushType.Stalagmite;
        public Camera mainCamera;

        [Header("Feedbacks")] [Tooltip("Feedbacks to play when the tool cannot interact with an object")]
        public MMFeedbacks cannotInteractFeedbacks;


        [Header("Allowed Layers")] [Tooltip("Allowed Unity layers for GameObjects (e.g., ore nodes)")]
        public LayerMask diggableLayers;

        [Tooltip("Allowed texture indices on terrain")]
        public int[] allowedTerrainTextureIndices;

        [SerializeField] protected ToolType toolType;
        [SerializeField] protected ToolIteration toolIteration;
        [SerializeField] protected MMFeedbacks equipFeedbacks;

        [FormerlySerializedAs("textureDetector")] [FormerlySerializedAs("_textureDetector")] [SerializeField]
        protected TerrainLayerDetector terrainLayerDetector;

        protected readonly ActionType Action = ActionType.Dig;
        protected readonly bool EditAsynchronously = true;
        private readonly float _baseMiningCooldown = 1f; // seconds between digs
        protected Coroutine CooldownCoroutine;

        private float currentDepth;

        // Debug logging
        protected bool debugLogging = true;

        protected DiggerMasterRuntime digger;
        protected float lastDigTime = -999f;
        protected RaycastHit lastHit;
        protected PlayerInteraction playerInteraction;

        protected void OnEnable()
        {
            this.MMEventStartListening();
        }

        protected void OnDisable()
        {
            this.MMEventStopListening();
        }

        public ToolType ToolType => toolType;
        public ToolIteration ToolIteration => toolIteration;
        public MMFeedbacks EquipFeedbacks => equipFeedbacks;

        public abstract void UseTool(RaycastHit hit);

        public abstract void PerformToolAction();

        public bool CanInteractWithTextureIndex(int index)
        {
            foreach (var allowed in allowedTerrainTextureIndices)
                if (index == allowed)
                    return true;
            return false;
        }

        public bool CanInteractWithObject(GameObject target)
        {
            return (diggableLayers.value & (1 << target.layer)) != 0;
        }

        public int GetCurrentTextureIndex()
        {
            try
            {
                var forwardTextureIndex = terrainLayerDetector.GetTextureIndex(lastHit, out _);

                // Early exit if invalid texture index
                if (forwardTextureIndex < 0)
                {
                    if (debugLogging) UnityEngine.Debug.Log($"Invalid texture index: {forwardTextureIndex}");
                    return 0; // Default safe value
                }

                var terrainBehavior = TerrainController.Instance.terrainBehavior;
                if (terrainBehavior == null)
                {
                    UnityEngine.Debug.LogError("TerrainBehavior is null");
                    return 0; // Default safe value
                }

                var digDepth = lastHit.point.y;
                if (float.IsNaN(digDepth) || float.IsInfinity(digDepth))
                {
                    UnityEngine.Debug.LogError($"Invalid dig depth: {digDepth}");
                    digDepth = 0; // Safe default
                }

                foreach (var defaultLayer in terrainBehavior.defaultLayerAboveDepths)
                {
                    if (defaultLayer == null) continue;

                    if (digDepth >= defaultLayer.playerDepth)
                    {
                        if (defaultLayer.alternateAcceptableLayerIndices != null)
                            foreach (var acceptableLayer in defaultLayer.alternateAcceptableLayerIndices)
                                if (acceptableLayer == forwardTextureIndex)
                                    return ConsiderTerrainChoices(acceptableLayer);

                        return ConsiderTerrainChoices(defaultLayer.defaultLayerIndex);
                    }
                }

                return ConsiderTerrainChoices(forwardTextureIndex);
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"Error in GetCurrentTextureIndex: {ex.Message}");
                return 0; // Default safe value
            }
        }

        public void HideCooldownBar()
        {
        }

        public void OnMMEvent(PowerUpEvent eventType)
        {
            var powerUpType = eventType.PowerUpScriptableObject.powerUpType;
            var powerUpMultiplier = eventType.PowerUpScriptableObject.multiplier;
            if (eventType.PowerUpEventType == PowerUpEventType.PowerUpActivated)
                if (powerUpType == PowerUpType.SpeedBoost)
                    miningCooldown /= powerUpMultiplier;

            if (eventType.PowerUpEventType == PowerUpEventType.PowerUpExpired)
                if (powerUpType == PowerUpType.SpeedBoost)
                    miningCooldown = _baseMiningCooldown;
        }

        protected int GetTerrainLayerBasedOnDepthAndOverrides(int rawTextureIndex, float digDepth)
        {
            try
            {
                // Validate input
                if (rawTextureIndex < 0)
                {
                    if (debugLogging) UnityEngine.Debug.Log($"Invalid raw texture index: {rawTextureIndex}");
                    return 0; // Default safe value
                }

                if (float.IsNaN(digDepth) || float.IsInfinity(digDepth))
                {
                    UnityEngine.Debug.LogError($"Invalid dig depth: {digDepth}");
                    digDepth = 0; // Safe default
                }

                var terrainBehavior = TerrainController.Instance?.terrainBehavior;
                if (terrainBehavior == null)
                {
                    UnityEngine.Debug.LogError("TerrainBehavior is null");
                    return 0; // Default safe value
                }

                if (terrainBehavior.defaultLayerAboveDepths == null)
                {
                    UnityEngine.Debug.LogError("defaultLayerAboveDepths is null");
                    return 0; // Default safe value
                }

                foreach (var defaultLayer in terrainBehavior.defaultLayerAboveDepths)
                {
                    if (defaultLayer == null) continue;

                    if (digDepth >= defaultLayer.playerDepth)
                    {
                        if (defaultLayer.alternateAcceptableLayerIndices != null)
                            foreach (var acceptableLayer in defaultLayer.alternateAcceptableLayerIndices)
                                if (acceptableLayer == rawTextureIndex)
                                    return ConsiderTerrainChoices(acceptableLayer);

                        return ConsiderTerrainChoices(defaultLayer.defaultLayerIndex);
                    }
                }

                return ConsiderTerrainChoices(rawTextureIndex);
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"Error in GetTerrainLayerBasedOnDepthAndOverrides: {ex.Message}");
                return 0; // Default safe value
            }
        }

        private int ConsiderTerrainChoices(int forwardTextureIndex)
        {
            try
            {
                if (forwardTextureIndex < 0)
                {
                    UnityEngine.Debug.LogError($"Invalid forward texture index: {forwardTextureIndex}");
                    return 0; // Default safe value
                }

                var terrainBehavior = TerrainController.Instance?.terrainBehavior;
                if (terrainBehavior == null || terrainBehavior.terrainChoices == null) return forwardTextureIndex;

                foreach (var terrainChoice in terrainBehavior.terrainChoices)
                {
                    if (terrainChoice == null) continue;

                    if (terrainChoice.terrainLayerIndex == forwardTextureIndex)
                        return terrainChoice.terrainToUseInstead;
                }

                return forwardTextureIndex;
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"Error in ConsiderTerrainChoices: {ex.Message}");
                return 0; // Default safe value
            }
        }

        public void SetDiggerUsingToolEffectSize(float newEffectRadius, float newEffectOpacity)
        {
            try
            {
                // Validate inputs for NaN
                if (float.IsNaN(newEffectRadius) || float.IsInfinity(newEffectRadius))
                {
                    UnityEngine.Debug.LogError($"Invalid effect radius: {newEffectRadius}");
                    newEffectRadius = minEffectRadius;
                }

                if (float.IsNaN(newEffectOpacity) || float.IsInfinity(newEffectOpacity))
                {
                    UnityEngine.Debug.LogError($"Invalid effect opacity: {newEffectOpacity}");
                    newEffectOpacity = minEffectOpacity;
                }

                // Apply safety limits
                effectRadius = Mathf.Clamp(newEffectRadius, minEffectRadius, maxEffectRadius);
                effectOpacity = Mathf.Clamp(newEffectOpacity, minEffectOpacity, maxEffectOpacity);

                // Log the assigned size for debugging
                // if (debugLogging)
                //     UnityEngine.Debug.Log($"DiggerUsingTool.size set to: {effectRadius}, {effectOpacity}");
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"Error in SetDiggerUsingToolEffectSize: {ex.Message}");
            }
        }

        protected void TriggerDebrisEffect(GameObject debrisEffectPrefab, RaycastHit hit)
        {
            try
            {
                // Validate inputs
                if (debrisEffectPrefab == null)
                    return;

                if (!IsValidVector3(hit.point) || !IsValidVector3(hit.normal))
                {
                    UnityEngine.Debug.LogError(
                        $"Invalid hit data for debris effect: point={hit.point}, normal={hit.normal}");
                    return;
                }

                // Debris FX
                var pos = hit.point + hit.normal * 0.1f;

                // Check if camera reference is valid
                if (mainCamera == null)
                {
                    UnityEngine.Debug.LogError("mainCamera is null for debris effect");
                    return;
                }

                var rot = Quaternion.LookRotation(-SanitizeVector3(mainCamera.transform.forward));
                var fx = Instantiate(debrisEffectPrefab, pos, rot);
                Destroy(fx, 2f);
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"Error in TriggerDebrisEffect: {ex.Message}");
            }
        }

        // Safe wrapper for Digger.Modify that handles NaN values
        protected bool SafeModify(Vector3 position, BrushType brushType, ActionType actionType,
            int textureIndex, float opacity, float radius, float height = 100f, bool smooth = true)
        {
            try
            {
                if (digger == null)
                {
                    UnityEngine.Debug.LogError("Digger reference is null");
                    return false;
                }

                // Validate and sanitize all parameters
                var safePosition = SanitizeVector3(position);
                if (!IsValidVector3(safePosition))
                {
                    UnityEngine.Debug.LogError($"Cannot use invalid position: {position}");
                    return false;
                }

                var safeOpacity = ClampFloat(opacity, 0.1f, 255f);
                var safeRadius = ClampFloat(radius, 0.1f, 10f);
                var safeHeight = ClampFloat(height, 0.1f, 1000f);


                // Call Digger's modify with safe parameters
                try
                {
                    if (EditAsynchronously)
                        digger.ModifyAsyncBuffured(
                            safePosition,
                            brushType,
                            actionType,
                            textureIndex,
                            safeOpacity,
                            safeRadius,
                            safeHeight,
                            smooth
                        );
                    else
                        digger.Modify(
                            safePosition,
                            brushType,
                            actionType,
                            textureIndex,
                            safeOpacity,
                            safeRadius,
                            safeHeight,
                            smooth
                        );

                    return true;
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogError($"Error in Digger.Modify: {ex.Message}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"Error in SafeModify: {ex.Message}");
                return false;
            }
        }

        // Utility methods to handle NaN and infinity values

        protected bool IsValidFloat(float value)
        {
            return !float.IsNaN(value) && !float.IsInfinity(value);
        }

        protected float ClampFloat(float value, float min, float max)
        {
            if (float.IsNaN(value) || float.IsInfinity(value))
                return min;
            return Mathf.Clamp(value, min, max);
        }

        protected bool IsValidVector3(Vector3 vector)
        {
            return IsValidFloat(vector.x) && IsValidFloat(vector.y) && IsValidFloat(vector.z);
        }

        protected Vector3 SanitizeVector3(Vector3 vector)
        {
            return new Vector3(
                float.IsNaN(vector.x) || float.IsInfinity(vector.x) ? 0f : vector.x,
                float.IsNaN(vector.y) || float.IsInfinity(vector.y) ? 0f : vector.y,
                float.IsNaN(vector.z) || float.IsInfinity(vector.z) ? 0f : vector.z
            );
        }
    }
}