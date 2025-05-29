using Domains.Gameplay.Mining.Scripts;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace Domains.Scripts_that_Need_Sorting
{
    public class JumpBehavior : MonoBehaviour
    {
        public MyNormalMovement myNormalMovement;


        private MMFeedbacks jumpFeedbacks;

        private void Awake()
        {
            if (myNormalMovement == null) myNormalMovement = GetComponentInParent<MyNormalMovement>();

            if (myNormalMovement == null) UnityEngine.Debug.LogError("MyNormalMovement not found in parent");

            myNormalMovement.OnJumpPerformed += OnJump;

            jumpFeedbacks = GetComponent<MMF_Player>();
        }

        public void OnJump()
        {
            jumpFeedbacks?.PlayFeedbacks();
        }
    }
}