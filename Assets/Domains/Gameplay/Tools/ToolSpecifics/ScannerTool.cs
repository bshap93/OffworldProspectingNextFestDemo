using System;
using CompassNavigatorPro;
using Domains.Gameplay.Equipment.Events;
using Domains.Gameplay.Equipment.Scripts;
using Domains.Scripts_that_Need_Sorting;
using HighlightPlus;
using MoreMountains.Feedbacks;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;
using UnityEngine.Serialization;

namespace Domains.Gameplay.Tools.ToolSpecifics
{
    public class ScannerTool : MonoBehaviour, IToolAction
    {
        [SerializeField] private ToolType toolType;
        [SerializeField] private ToolIteration toolIteration;
        [SerializeField] private MMFeedbacks equipFeedbacks;
        [SerializeField] private MMFeedbacks useFeedbacks;
        [SerializeField] private HighlightEffect highlightEffect;
        [SerializeField] protected float scanningCooldown = 3f;
        Coroutine CooldownCoroutine;
        protected float lastScanTime = -999f;

        [SerializeField] ProgressBarBlue cooldownProgressBar;

        [FormerlySerializedAs("textureDetector")] [SerializeField]
        private TerrainLayerDetector terrainLayerDetector;
         CompassPro _compassNavigatorPro;

        [SerializeField] private LayerMask playerMask;
        [SerializeField] private float maxToolRange = 5f;
        [SerializeField] private Camera mainCamera;
        private RaycastHit lastHit;

        public ToolType ToolType => toolType;
        public ToolIteration ToolIteration => toolIteration;
        public MMFeedbacks EquipFeedbacks => equipFeedbacks;

        private void Start()
        {
            _compassNavigatorPro = FindFirstObjectByType<CompassPro>();
            if (_compassNavigatorPro == null) 
                UnityEngine.Debug.LogWarning("CompassPro component not found in the scene.");
        }


        public void UseTool(RaycastHit hit)
        {
            PerformToolAction();
        }

        public void PerformToolAction()
        {
            if (Time.time < lastScanTime + scanningCooldown)
                return;
            
            lastScanTime = Time.time;
            if (CooldownCoroutine != null)
            {
                StopCoroutine(CooldownCoroutine);
            }
            EquipmentEvent.Trigger(EquipmentEventType.ChangeToEquipment, ToolType.Scanner);
            useFeedbacks?.PlayFeedbacks();
            highlightEffect.highlighted = true;
            _compassNavigatorPro.Scan();
            
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
    }
}