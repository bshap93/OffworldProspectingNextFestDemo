using Domains.Effects.Scripts;
using Domains.Gameplay.Equipment.Events;
using Domains.Gameplay.Managers.Scripts;
using Domains.Gameplay.Tools;
using Domains.Input.Scripts;
using Domains.Scripts_that_Need_Sorting;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Serialization;

namespace Domains.Gameplay.Equipment.Scripts
{
    public class PlayerEquipment : MonoBehaviour
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

            // Convert MonoBehaviours to IToolAction
            Tools = new IToolAction[toolObjects.Length];
            for (var i = 0; i < toolObjects.Length; i++)
            {
                Tools[i] = toolObjects[i].GetComponent<IToolAction>();
                if (Tools[i] == null) UnityEngine.Debug.LogError($"Tool at index {i} does not implement IToolAction.");
            }

            EquipmentEvent.Trigger(EquipmentEventType.ChangeToEquipment, currentToolType);
        }

        private void Start()
        {
            numTools = Tools.Length;
            CurrentToolComponent = Tools[0];
            toolPanelController.ActivateToolPanelItem(currentToolType);
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