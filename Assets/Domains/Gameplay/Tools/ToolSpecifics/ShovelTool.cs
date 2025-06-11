using System.Linq;
using Digger.Modules.Runtime.Sources;
using Domains.Gameplay.Managers;
using Domains.Gameplay.Mining.Scripts;
using Domains.Gameplay.Objects.Powerups;
using Domains.Player.Camera;
using Domains.Player.Events;
using Domains.Player.Scripts;
using Domains.Scripts_that_Need_Sorting;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using UnityEngine;

namespace Domains.Gameplay.Tools.ToolSpecifics
{
    public class ShovelTool : BaseDiggerUsingTool, MMEventListener<UpgradeEvent>
    {
        [Header("Feedbacks")] public MMFeedbacks diggingFeedbacks;

        [SerializeField] private ProgressBarBlue cooldownProgressBar;
        public GameObject debrisEffectPrefab;

        [Header("Material")] [Header("Material Settings")]
        public Material currentMaterial;

        [Header("Shovel Models")] [SerializeField]
        private GameObject shovelObject;

        [SerializeField] private GameObject shovelGripObject;
        [SerializeField] private GameObject shovelMidObject;

        private void Awake()
        {
            digger = FindFirstObjectByType<DiggerMasterRuntime>();
            playerInteraction = FindFirstObjectByType<PlayerInteraction>();
        }

        private new void OnEnable()
        {
            base.OnEnable();
            this.MMEventStartListening<UpgradeEvent>();
            // Reapply current material when enabled
            if (currentMaterial != null) SetCurrentMaterial(currentMaterial);
        }

        private new void OnDisable()
        {
            base.OnDisable();
            this.MMEventStopListening<UpgradeEvent>();
        }


        public void OnMMEvent(UpgradeEvent eventType)
        {
            if (eventType.EventType == UpgradeEventType.ShovelMiningSizeSet)
                SetDiggerUsingToolEffectSize(eventType.EffectValue, eventType.EffectValue2);
        }

        public override void OnMMEvent(PowerUpEvent eventType)
        {
            base.OnMMEvent(eventType);
        }

        public void SetCurrentMaterial(Material material)
        {
            currentMaterial = material;
            if (shovelObject != null)
                shovelObject.GetComponent<Renderer>().material = currentMaterial;
            if (shovelGripObject != null)
                shovelGripObject.GetComponent<Renderer>().material = currentMaterial;
            if (shovelMidObject != null)
                shovelMidObject.GetComponent<Renderer>().material = currentMaterial;
        }


        public override void UseTool(RaycastHit hit)
        {
            lastHit = hit;
            PerformToolAction();
        }


        public override void PerformToolAction()
        {
            if (Time.time < lastDigTime + miningCooldown)
                return;
            var detectedTextureIndex = terrainLayerDetector.GetTextureIndex(lastHit, out _);

// Reject early if not in allowed textures (raw index)
            if (!CanInteractWithTextureIndex(detectedTextureIndex)) return;

// Determine final texture to dig into
            var textureIndex = GetTerrainLayerBasedOnDepthAndOverrides(detectedTextureIndex, lastHit.point.y);


            lastDigTime = Time.time;
            if (CooldownCoroutine != null)
                StopCoroutine(CooldownCoroutine);

            // CooldownCoroutine = StartCoroutine(ShowCooldownBarCoroutine(miningCooldown));

            if (playerInteraction == null || digger == null)
                return;


            var notPlayerMask = ~playerInteraction.playerLayerMask;
            if (!Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out var hit,
                    diggerUsingRange,
                    notPlayerMask))
                return;

            // Cache hit for external access
            lastHit = hit;

            // Interact
            if (CanInteractWithObject(hit.collider.gameObject))
            {
                // Call IInteractable if implemented
                hit.collider.GetComponent<IInteractable>()?.Interact();


                var minable = hit.collider.GetComponent<IMinable>();
                if (minable != null)
                {
                    minable.MinableFailHit(hit.point);
                    moveToolDespiteFailHitFeedbacks?.PlayFeedbacks();
                    CameraEffectEvent.Trigger(CameraEffectEventType.ShakeCameraPosition, 0.2f);
                    // CooldownCoroutine = StartCoroutine(ShowCooldownBarCoroutine(miningCooldown));

                    return;
                }
            }

            // Return after triggering failed mining feedbacks, and before digging
            if (!allowedTerrainTextureIndices.Contains(detectedTextureIndex)) return;

            GameObject prefabToUse = null;
            foreach (var terDigPrefab in TerrainController.Instance.terrainBehavior.terrainDigParticlePrefabs)
                if (terDigPrefab.terrainLayerIndex == detectedTextureIndex)
                {
                    prefabToUse = terDigPrefab.primaryPrefab;
                    break;
                }

            ScannerEvent.Trigger(ScannerEventType.ScannerCalibrated, "Calibrating: No Target");


            if (prefabToUse != null)
            {
                UnityEngine.Debug.Log("Prefab to use found: " + prefabToUse.name);

                TriggerDebrisEffect(prefabToUse, hit);
            }
            else
            {
                TriggerDebrisEffect(debrisEffectPrefab, hit);
            }


            // Feedback trigger (from PerformToolAction, not MMFeedbacks directly)
            if (diggingFeedbacks != null) diggingFeedbacks.PlayFeedbacks(hit.point);


            // Dig!
            var digPosition = hit.point + mainCamera.transform.forward * 0.3f;

            var didDig = false;

            if (EditAsynchronously)
                didDig = digger.ModifyAsyncBuffured(digPosition, brush, Action, textureIndex, effectOpacity,
                    effectRadius,
                    stalagmiteHeight);
            else
                digger.Modify(digPosition, brush, Action, textureIndex, effectOpacity, effectRadius);

            if (didDig)
                CooldownCoroutine =
                    StartCoroutine(cooldownProgressBar.ShowCooldownBarCoroutine(miningCooldown));

            // Experimental gravity integration
            // Check nearby gravity objects
            SimpleGravityIntegration.OnDigPerformed(digPosition, effectRadius * 2f);
        }
    }
}