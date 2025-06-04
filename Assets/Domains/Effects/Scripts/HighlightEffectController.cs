using Domains.Gameplay.Equipment.Events;
using Domains.Gameplay.Equipment.Scripts;
using Domains.Scripts_that_Need_Sorting;
using HighlightPlus;
using MoreMountains.Tools;
using Plugins.Kronnect.HighlightPlus.Scripts;
using UnityEngine;
using UnityEngine.Events;

namespace Domains.Effects.Scripts
{
    public class HighlightEffectController : MonoBehaviour, MMEventListener<EquipmentEvent>
    {
        [SerializeField] public string targetID;

        public UnityEvent onScanned;
        [SerializeField] private float targetDuration = 3f;
        private HighlightTrigger _myMyHighlightTrigger;
        private HighlightEffect highlightEffect;

        private void Awake()
        {
            highlightEffect = GetComponent<HighlightEffect>();
            _myMyHighlightTrigger = GetComponent<HighlightTrigger>();
        }

        private void Start()
        {
            if (highlightEffect == null)
            {
                UnityEngine.Debug.LogError("HighlightEffect component not found on this GameObject.");
                return;
            }

            if (_myMyHighlightTrigger == null)
                UnityEngine.Debug.LogError("HighlightTrigger component not found on this GameObject.");


            ConfigureForTerrainObjects();

            SetSeeThroughMode(SeeThroughMode.Never);
        }

        private void OnEnable()
        {
            this.MMEventStartListening();
        }

        private void OnDisable()
        {
            this.MMEventStopListening();
        }

        public void OnMMEvent(EquipmentEvent eventType)
        {
            if (eventType.ToolType == ToolType.Scanner && eventType.EventType == EquipmentEventType.UseEquipment)
            {
                if (highlightEffect != null) SetSeeThroughMode(SeeThroughMode.AlwaysWhenOccluded);
            }
            else if (eventType.ToolType == ToolType.Pickaxe || eventType.ToolType == ToolType.Shovel)
            {
                if (highlightEffect != null) SetSeeThroughMode(SeeThroughMode.Never);
            }
        }


        // Simplified to not need a parameter since we already checked the targetID
        public void ActivateTarget()
        {
            if (highlightEffect != null) highlightEffect.targetFX = true;
            // After X seconds, set targetFX to false
        }


        public void TriggerHighlightEffect()
        {
            if (highlightEffect != null)
            {
                highlightEffect.highlighted = true;
                highlightEffect.Refresh();
            }
        }

        public void StopHighlightEffect()
        {
            if (highlightEffect != null)
            {
                highlightEffect.highlighted = false;
                highlightEffect.Refresh();
            }
        }

        public void SetSeeThroughMode(SeeThroughMode mode)
        {
            if (mode == SeeThroughMode.Never)
                if (highlightEffect != null)
                {
                    highlightEffect.seeThrough = SeeThroughMode.Never;
                    highlightEffect.Refresh();
                }

            // Add null check for PlayerEquipment.Instance
            if (PlayerEquipment.Instance == null)
            {
                UnityEngine.Debug.LogWarning("PlayerEquipment.Instance is null, cannot set see-through mode properly.");
                return;
            }

            var distance = GetDistanceFromPlayer();
            // Set at 5 for now
            if (distance < PlayerEquipment.Instance.GetScannerMaxRange())
                if (highlightEffect != null)
                {
                    onScanned?.Invoke();
                    if (highlightEffect.seeThrough != mode)
                    {
                        highlightEffect.seeThrough = mode;
                        highlightEffect.Refresh();
                    }
                }
            // var normalizedDist = distance / PlayerEquipment.Instance.scannerMaxRange;
        }

        public float GetDistanceFromPlayer()
        {
            if (highlightEffect != null)
                if (Camera.main != null)
                    return Vector3.Distance(highlightEffect.transform.position, Camera.main.transform.position);
            return 0f;
        }

        public void TriggerHighlightHitEffect()
        {
            highlightEffect.HitFX();
        }

        public void ConfigureForTerrainObjects()
        {
            if (highlightEffect != null)
            {
                // Enable ordered see-through for better terrain handling
                highlightEffect.seeThroughOrdered = true;

                // Set accurate rendering for terrains
                highlightEffect.seeThroughOccluderMaskAccurate = true;

                // Assign terrain to a specific layer if not already done
                if (GetComponent<Terrain>() != null)
                    gameObject.layer = LayerMask.NameToLayer("Terrain"); // Create this layer in your project
            }
        }
    }
}