using System;
using DG.Tweening;
using JetBrains.Annotations;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace Domains.Scripts_that_Need_Sorting
{
    public class HabDoorsController : MonoBehaviour
    {
        [Serializable]
        public enum DoorOpenDirection
        {
            Outward,
            Inward
        }

        public GameObject RDoor;
        public GameObject LDoor;

        public DoorOpenDirection DoorOpenDirectionType;

        [CanBeNull] public MMFeedbacks OpenDoorFeedbacks;
        [CanBeNull] public MMFeedbacks CloseDoorFeedbacks;
        private bool isOpen;
        private Quaternion originalRotationL;

        private Quaternion originalRotationR;

        private void Start()
        {
            // Store original rotations on Start
            originalRotationR = RDoor.transform.rotation;
            originalRotationL = LDoor.transform.rotation;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
                OpenDoors();
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
                CloseDoors();
        }

        public void OpenDoors()
        {
            if (isOpen) return;
            isOpen = true;
            if (OpenDoorFeedbacks != null) OpenDoorFeedbacks.PlayFeedbacks();

            if (DoorOpenDirectionType == DoorOpenDirection.Outward)
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

        public void CloseDoors()
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