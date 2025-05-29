using Domains.Scripts_that_Need_Sorting;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace Domains.Gameplay.Tools
{
    public interface IToolAction
    {
        ToolType ToolType { get; }
        ToolIteration ToolIteration { get; }

        MMFeedbacks EquipFeedbacks { get; }
        void UseTool(RaycastHit hit);
        void PerformToolAction();

        bool CanInteractWithTextureIndex(int index);
        bool CanInteractWithObject(GameObject target);

        int GetCurrentTextureIndex();

        void HideCooldownBar();
    }
}