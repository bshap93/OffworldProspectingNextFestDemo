using Domains.Gameplay.Equipment.Scripts;
using Domains.Gameplay.Mining.Events;
using UnityEngine;

namespace Domains.Scripts_that_Need_Sorting
{
    public class MiningBehavior : MonoBehaviour
    {
        [SerializeField] private ToolIteration toolIteration;
        [SerializeField] private ToolType toolType;

        private void Start()
        {
            GetTool();
        }

        private void GetTool()
        {
            toolIteration = PlayerEquipment.Instance.currentToolIteration;
            toolType = PlayerEquipment.Instance.currentToolType;
        }

        public void OnMining()
        {
            GetTool();
            switch (toolType)
            {
                case ToolType.Shovel:
                    switch (toolIteration)
                    {
                        case ToolIteration.First:
                            ToolEvent.Trigger(ToolEventType.UseTool, ToolType.Shovel, ToolIteration.First);
                            break;
                        case ToolIteration.Second:
                            ToolEvent.Trigger(ToolEventType.UseTool, ToolType.Shovel, ToolIteration.Second);
                            break;
                    }

                    break;

                case ToolType.Pickaxe:
                    switch (toolIteration)
                    {
                        case ToolIteration.First:
                            ToolEvent.Trigger(ToolEventType.UseTool, ToolType.Pickaxe, ToolIteration.First);
                            break;
                        case ToolIteration.Second:
                            ToolEvent.Trigger(ToolEventType.UseTool, ToolType.Pickaxe, ToolIteration.Second);
                            break;
                    }

                    break;
            }
        }
    }
}