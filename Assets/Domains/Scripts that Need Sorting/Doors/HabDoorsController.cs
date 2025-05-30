using System;
using DG.Tweening;
using JetBrains.Annotations;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Serialization;

namespace Domains.Scripts_that_Need_Sorting
{
    public class HabDoorsController: DoorsController
    {
        [Serializable]
        public enum DoorOpenDirection
        {
            Outward,
            Inward
        }



        [FormerlySerializedAs("DoorOpenDirectionType")] public DoorOpenDirection doorOpenDirectionType;



        private Quaternion originalRotationL;

        private Quaternion originalRotationR;

        private void Start()
        {
            // Store original rotations on Start
            originalRotationR = RDoor.transform.rotation;
            originalRotationL = LDoor.transform.rotation;
        }


        public override void OpenDoors()
        {
            if (isOpen) return;
            isOpen = true;
            if (OpenDoorFeedbacks != null) OpenDoorFeedbacks.PlayFeedbacks();

            if (doorOpenDirectionType == DoorOpenDirection.Outward)
            {
                // Rotate outward
                RDoor.transform.DORotate(originalRotationR.eulerAngles + new Vector3(0, -90, 0), 1);
                LDoor.transform.DORotate(originalRotationL.eulerAngles + new Vector3(0, 90, 0), 1);
            }
            else
            {
                // Rotate inwar
                RDoor.transform.DORotate(originalRotationR.eulerAngles + new Vector3(0, 90, 0), 1);
                LDoor.transform.DORotate(originalRotationL.eulerAngles + new Vector3(0, -90, 0), 1);
            }
        }

        public override void CloseDoors()
        {
            if (!isOpen) return;
            isOpen = false;
            if (CloseDoorFeedbacks != null) CloseDoorFeedbacks.PlayFeedbacks();

            // Reset to original rotation instead of arbitrary values
            RDoor.transform.DORotate(originalRotationR.eulerAngles, 1);
            LDoor.transform.DORotate(originalRotationL.eulerAngles, 1);
        }
    }
}