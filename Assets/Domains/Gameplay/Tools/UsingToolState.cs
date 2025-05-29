using Domains.Gameplay.Equipment.Scripts;
using Domains.Gameplay.Tools.ToolSpecifics;
using Domains.Input.Scripts;
using Domains.Scripts_that_Need_Sorting;
using ThirdParty.Character_Controller_Pro.Implementation.Scripts.Character.States;
using UnityEngine;
using UnityEngine.Serialization;

namespace Domains.Gameplay.Tools
{
    public class UsingToolState : CharacterState
    {
        public UnityEngine.Camera mainCamera;

        [FormerlySerializedAs("textureDetector")] [SerializeField]
        private TerrainLayerDetector terrainLayerDetector;

        [SerializeField] private LayerMask playerMask;
        [SerializeField] private float maxToolRange = 5f;


        public override void UpdateBehaviour(float dt)
        {
            // Check for tool usage input
            if (CustomInputBindings.IsMineMouseButtonPressed())
            {
                var tool = PlayerEquipment.Instance.CurrentToolComponent;
                if (tool == null) return;

                if (tool is ScannerTool)
                {
                    // Perform tool action
                    tool.PerformToolAction();
                    return;
                }

                var notPlayerMask = ~playerMask;
                if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out var hit,
                        maxToolRange, notPlayerMask))
                {
                    // Get terrain texture index at hit point
                    Terrain terrain;
                    var textureIndex = terrainLayerDetector.GetTextureIndex(hit, out terrain);

                    // Check if the tool supports both terrain and object
                    var canUse = tool.CanInteractWithTextureIndex(textureIndex) ||
                                 tool.CanInteractWithObject(hit.collider.gameObject);

                    if (canUse)
                        tool.UseTool(hit);
                    else
                        // Optional: play denied feedback
                        UnityEngine.Debug.Log("Tool not valid for this surface or object.");
                }
            }
        }
    }
}