using CompassNavigatorPro;
using Domains.Gameplay.Equipment.Events;
using Domains.Gameplay.Equipment.Scripts;
using Domains.Player.Events;
using Domains.Scripts_that_Need_Sorting;
using Domains.UI_Global.Events;
using HighlightPlus;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Serialization;

namespace Domains.Gameplay.Tools.ToolSpecifics
{
    public class ScannerTool : MonoBehaviour, IToolAction, MMEventListener<UpgradeEvent>
    {
        [SerializeField] private ToolType toolType;
        [SerializeField] private ToolIteration toolIteration;
        [SerializeField] private MMFeedbacks equipFeedbacks;
        [SerializeField] private MMFeedbacks useFeedbacks;
        [SerializeField] private HighlightEffect highlightEffect;
        [SerializeField] protected float scanningCooldown = 3f;

        public Material currentMaterial;


        [SerializeField] private ProgressBarBlue cooldownProgressBar;

        [FormerlySerializedAs("textureDetector")] [SerializeField]
        private TerrainLayerDetector terrainLayerDetector;

        [SerializeField] private LayerMask playerMask;
        [SerializeField] public float maxToolRange = 5f;
        [SerializeField] private Camera mainCamera;
        private CompassPro _compassNavigatorPro;
        private Coroutine CooldownCoroutine;
        private RaycastHit lastHit;
        protected float lastScanTime = -999f;
        private MeshRenderer meshRenderer;


        private void Start()
        {
            _compassNavigatorPro = FindFirstObjectByType<CompassPro>();
            if (_compassNavigatorPro == null)
                UnityEngine.Debug.LogWarning("CompassPro component not found in the scene.");

            meshRenderer = GetComponent<MeshRenderer>();
            if (currentMaterial != null) SetCurrentMaterial(currentMaterial);
        }

        public void OnEnable()
        {
            this.MMEventStartListening();
        }

        public void OnDisable()
        {
            this.MMEventStopListening();
        }

        public ToolType ToolType => toolType;
        public ToolIteration ToolIteration => toolIteration;
        public MMFeedbacks EquipFeedbacks => equipFeedbacks;


        public void UseTool(RaycastHit hit)
        {
            PerformToolAction();
        }

        public void PerformToolAction()
        {
            if (Time.time < lastScanTime + scanningCooldown)
                return;

            lastScanTime = Time.time;
            if (CooldownCoroutine != null) StopCoroutine(CooldownCoroutine);
            EquipmentEvent.Trigger(EquipmentEventType.UseEquipment, ToolType.Scanner);
            useFeedbacks?.PlayFeedbacks();
            highlightEffect.highlighted = true;
            _compassNavigatorPro.Scan();
            UnityEngine.Debug.Log("Scanned with range " + maxToolRange);

            CooldownCoroutine = StartCoroutine(cooldownProgressBar.ShowCooldownBarCoroutine(scanningCooldown));
        }

        public bool CanInteractWithTextureIndex(int index)
        {
            return false;
        }


        public bool CanInteractWithObject(GameObject target)
        {
            return false;
        }

        public int GetCurrentTextureIndex()
        {
            var notPlayerMask = ~playerMask;
            if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out var hit,
                    maxToolRange, notPlayerMask))
            {
                var tool = PlayerEquipment.Instance.CurrentToolComponent;
                if (tool == null) return -1;

                // Get terrain texture index at hit point
                Terrain terrain;
                var textureIndex = terrainLayerDetector.GetTextureIndex(hit, out terrain);

                return textureIndex;
            }

            return -1;
        }

        public void HideCooldownBar()
        {
        }


        public void OnMMEvent(UpgradeEvent eventType)
        {
            if (eventType.EventType == UpgradeEventType.ScannerRangeSet) SetScannerRange(eventType.EffectValue);
        }

        public void SetScannerRange(float newScannerRange)
        {
            maxToolRange = newScannerRange;
            AlertEvent.Trigger(AlertReason.ScanningRangeIncreased, "Scanner range increased to " + newScannerRange);
        }

        public void SetCurrentMaterial(Material upgradeMaterial)
        {
            currentMaterial = upgradeMaterial;
            if (meshRenderer != null)
                meshRenderer.material = upgradeMaterial;
        }
    }
}