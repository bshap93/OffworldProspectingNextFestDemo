using Domains.Gameplay.Equipment.Scripts;
using Domains.Gameplay.Mining.Scripts;
using Domains.Gameplay.Tools;
using Domains.Gameplay.Tools.ToolSpecifics;
using Domains.Scripts_that_Need_Sorting;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.UI;

namespace Domains.UI_Global.Reticle
{
    public class ReticleController : MonoBehaviour, MMEventListener<ScannerEvent>
    {
        [Header("Reticle States")] public ReticleState defaultState;

        public ReticleState interactableState;
        public ReticleState mineableState;
        public ReticleState switchToolState;
        public ReticleState validTerrainState;
        public ReticleState scannerState;
        public ReticleState scannerNotCalibratedState;


        public TerrainLayerDetector terrainLayerDetector;
        [Header("Reticle UI")] public Image reticle;

        private ReticleState currentState;
        private IToolAction currentTool;

        private bool scannerCalibrated;

        private void OnEnable()
        {
            this.MMEventStartListening();
        }

        private void OnDisable()
        {
            this.MMEventStopListening();
        }

        public void OnMMEvent(ScannerEvent eventType)
        {
            if (eventType.ScannerEventType == ScannerEventType.ScannerCalibrated)
            {
                scannerCalibrated = true;
                ApplyReticleState(scannerState);
                UnityEngine.Debug.unityLogger.Log(eventType.ScannerEventType);
            }
        }

        public void UpdateReticle(RaycastHit? hit, bool terrainBlocking)
        {
            var targetState = defaultState;
            currentTool = PlayerEquipment.Instance.CurrentToolComponent;

            // Special case for Scanner tool - always use scanner state
            if (currentTool is ScannerTool)
            {
                if (scannerCalibrated)
                    ApplyReticleState(scannerState);
                else
                    ApplyReticleState(scannerNotCalibratedState);

                return;
            }

            if (hit.HasValue)
            {
                var terrainIndex = terrainLayerDetector.textureIndex;

                // Check for interactable components
                var interactable = hit.Value.collider.GetComponent<IInteractable>();
                var minable = hit.Value.collider.GetComponent<IMinable>();

                if (!terrainBlocking)
                {
                    if (interactable != null)
                    {
                        targetState = interactableState;
                    }
                    else if (minable != null)
                    {
                        // Check if current tool can mine this object
                        var pickaxe = currentTool as PickaxeTool;
                        if (pickaxe != null && pickaxe.hardnessCanBreak >= minable.GetCurrentMinableHardness()
                           )
                        {
                            targetState = mineableState;
                        }
                        else if (pickaxe != null && pickaxe.hardnessCanBreak >= minable.GetCurrentMinableHardness())
                        {
                            targetState = defaultState;
                        }
                        else
                        {
                            // Check if any tool can mine this
                            var anyToolCanMine = false;
                            foreach (var tool in PlayerEquipment.Instance.Tools)
                                if (tool is PickaxeTool tempPickaxe && tempPickaxe.hardnessCanBreak >=
                                    minable.GetCurrentMinableHardness())
                                {
                                    anyToolCanMine = true;
                                    break;
                                }

                            targetState = anyToolCanMine ? switchToolState : defaultState;
                        }
                    }
                }

                // Check terrain if no interactable/minable takes precedence
                if (targetState == defaultState && terrainIndex >= 0 && currentTool != null)
                {
                    // Check if current tool can dig this terrain
                    if (currentTool.CanInteractWithTextureIndex(terrainIndex))
                        targetState = validTerrainState;
                    else
                        // Check if another tool can dig this terrain
                        foreach (var tool in PlayerEquipment.Instance.Tools)
                            if (tool != currentTool && tool.CanInteractWithTextureIndex(terrainIndex))
                            {
                                targetState = switchToolState;
                                break;
                            }
                }
            }

            ApplyReticleState(targetState);
        }

        private void ApplyReticleState(ReticleState state)
        {
            if (state != currentState)
            {
                currentState = state;
                if (state != null)
                {
                    reticle.sprite = state.reticleSprite;
                    reticle.color = state.reticleColor;
                }
                else
                {
                    reticle.sprite = defaultState.reticleSprite;
                    reticle.color = defaultState.reticleColor;
                }
            }
        }
    }
}