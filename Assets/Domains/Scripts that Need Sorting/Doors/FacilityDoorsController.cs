using DG.Tweening;
using JetBrains.Annotations;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace Domains.Scripts_that_Need_Sorting
{
    public class FacilityDoorsController: DoorsController
    {


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

            LDoor.transform.DOLocalMoveX(  1.5f,  1f);
            RDoor.transform.DOLocalMoveX(-1.5f, 1f);
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