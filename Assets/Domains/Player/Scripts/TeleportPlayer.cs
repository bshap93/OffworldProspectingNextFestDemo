using Lightbug.CharacterControllerPro.Core;
using Lightbug.Utilities;
using UnityEngine;

namespace Domains.Player.Scripts
{
    public class TeleportPlayer : MonoBehaviour
    {
        public enum RotationMode
        {
            /// <summary>
            /// </summary>
            ModifyUp,

            /// <summary>
            /// </summary>
            AlignWithObject
        }


        [Condition("rotationMode", ConditionAttribute.ConditionType.IsEqualTo)]
        [Tooltip("The target Transform.up vector to use.")]
        public Transform referenceTransform;

        [Header("Rotation")] public bool rotate;

        [Condition("rotate", ConditionAttribute.ConditionType.IsTrue)]
        public RotationMode rotationMode = RotationMode.ModifyUp;

        [Header("Position")] public bool teleport;

        [Condition("teleport", ConditionAttribute.ConditionType.IsTrue, ConditionAttribute.VisibilityType.NotEditable)]
        public Transform teleportTarget;

        [Condition(
            new[] { "rotationMode", "rotate" },
            new[] { ConditionAttribute.ConditionType.IsEqualTo, ConditionAttribute.ConditionType.IsTrue },
            new[] { (int)RotationMode.AlignWithObject, 0f })]
        public VerticalAlignmentSettings.VerticalReferenceMode upDirectionReferenceMode =
            VerticalAlignmentSettings.VerticalReferenceMode.Away;

        [Condition(
            new[] { "rotationMode", "rotate" },
            new[] { ConditionAttribute.ConditionType.IsEqualTo, ConditionAttribute.ConditionType.IsTrue },
            new[] { (int)RotationMode.AlignWithObject, 0f })]
        [Tooltip("The target transform to use as the reference.")]
        public Transform verticalAlignmentReference;

        public void Teleport(CharacterActor characterActor)
        {
            if (!teleport)
                return;

            if (teleportTarget == null)
                return;

            var targetPosition = teleportTarget.position;

            // If the character is 2D, don't change the position z component (Transform).
            if (characterActor.Is2D)
                targetPosition.z = characterActor.transform.position.z;

            characterActor.Teleport(targetPosition);
        }

        private void Rotate(CharacterActor characterActor)
        {
            if (!rotate)
                return;

            switch (rotationMode)
            {
                case RotationMode.ModifyUp:

                    if (referenceTransform != null)
                        characterActor.Up = referenceTransform.up;

                    if (characterActor.constraintRotation)
                    {
                        characterActor.upDirectionReference = null;
                        characterActor.constraintUpDirection = characterActor.Up;
                    }

                    break;
                case RotationMode.AlignWithObject:

                    // Just in case the rotation constraint is active ...
                    characterActor.constraintRotation = true;
                    characterActor.upDirectionReference = verticalAlignmentReference;
                    characterActor.upDirectionReferenceMode = upDirectionReferenceMode;
                    characterActor.constraintUpDirection = characterActor.Up;
                    break;
            }
        }
    }
}