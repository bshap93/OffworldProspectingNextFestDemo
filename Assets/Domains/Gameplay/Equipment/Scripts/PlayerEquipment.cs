using Domains.Effects.Scripts;
using Domains.Gameplay.Equipment.Events;
using Domains.Gameplay.Managers.Scripts;
using Domains.Gameplay.Mining.Scripts;
using Domains.Gameplay.Tools;
using Domains.Gameplay.Tools.ToolSpecifics;
using Domains.Input.Scripts;
using Domains.Scripts_that_Need_Sorting;
using Domains.UI_Global.Events;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Serialization;

namespace Domains.Gameplay.Equipment.Scripts
{
    public class PlayerEquipment : MonoBehaviour, MMEventListener<ScannerEvent>
    {
        public static PlayerEquipment Instance;


        [SerializeField] private MMFeedbacks equipMinerFeedbacks;
        [SerializeField] private MMFeedbacks equipScannerFeedbacks;
        [SerializeField] private ToolPanelController toolPanelController;

        [FormerlySerializedAs("ScannerMaxRange")] [SerializeField]
        public float scannerMaxRange = 5f;

        public ToolType currentToolType;
        public ToolIteration currentToolIteration;

        [SerializeField] private int currentToolIndex;

        [FormerlySerializedAs("toolBehaviours")] [SerializeField]
        private GameObject[] toolObjects; // Shown in Inspector

        [SerializeField] private float toolSwitchCooldown = 0.3f;

        public IToolAction CurrentToolComponent;
        private float lastToolSwitchTime = -1f;

        private int numTools;

        public IToolAction[] Tools { get; private set; } // Used in code


        private void Awake()
        {
            Instance = this;

            Tools = new IToolAction[toolObjects.Length];

            for (var i = 0; i < toolObjects.Length; i++)
            {
                toolObjects[i].SetActive(true); // Temporarily enable
                Tools[i] = toolObjects[i].GetComponent<IToolAction>();
                toolObjects[i].SetActive(false); // Immediately disable again
            }

            EquipmentEvent.Trigger(EquipmentEventType.ChangeToEquipment, currentToolType);
        }

        private void Start()
        {
            numTools = Tools.Length;
            SwitchTool(currentToolIndex);
            // CurrentToolComponent = Tools[0];

            toolPanelController.ActivateToolPanelItem(currentToolType);

            UnityEngine.Debug.Log("Initial tool is: " + CurrentToolComponent?.GetType().Name);
        }

        private void Update()
        {
            if (CustomInputBindings.IsChangingTools() && Time.time - lastToolSwitchTime > toolSwitchCooldown)
            {
                if (PauseManager.Instance.IsPaused()) return;
                var direction = CustomInputBindings.GetWeaponChangeDirection();
                currentToolIndex = (currentToolIndex + direction + numTools) % numTools;
                SwitchTool(currentToolIndex);
                lastToolSwitchTime = Time.time;
            }
        }

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
                var scannerTool = Tools[2] as ScannerTool;
                if (scannerTool != null && scannerTool.calibrated == false)
                {
                    scannerTool.calibrated = true; // Assuming ScannerTool is at index 2
                    AlertEvent.Trigger(AlertReason.ScannerCalibrated,
                        "Scanner calibrated successfully. You can now conduct scans.", "Scanner Calibrated");
                }
                else
                {
                    UnityEngine.Debug.LogWarning("ScannerTool not found at index 2.");
                }
            }
        }

        private void SwitchTool(int index)
        {
            if (CurrentToolComponent == null)
                UnityEngine.Debug.LogWarning($"Tool at index {index} is missing an IToolAction component.");

            if (CurrentToolComponent != null) CurrentToolComponent.HideCooldownBar(); // 🔧 ADD THIS
            currentToolIndex = index;

            var tool = Tools[index];

            if (tool == null)
            {
                UnityEngine.Debug.LogWarning($"Tool at index {index} is null.");
                return;
            }

            currentToolType = tool.ToolType;
            currentToolIteration = tool.ToolIteration;
            CurrentToolComponent = tool;

            toolPanelController.ActivateToolPanelItem(currentToolType);

            // Disable all tools
            foreach (var t in Tools)
                if (t is MonoBehaviour monoBehaviour)
                {
                    var go = monoBehaviour.gameObject;
                    if (go.activeSelf) FadeUtils.FadeOut(go); // Fade before disable
                    go.SetActive(false); // Still necessary to avoid interactions
                }

            if (tool is MonoBehaviour mbh)
            {
                mbh.gameObject.SetActive(true);
                FadeUtils.FadeIn(mbh.gameObject); // Smooth appearance
            }

            // Trigger the appropriate events and feedbacks
            EquipmentEvent.Trigger(EquipmentEventType.ChangeToEquipment, currentToolType);
            tool.EquipFeedbacks?.PlayFeedbacks();
        }
    }
}