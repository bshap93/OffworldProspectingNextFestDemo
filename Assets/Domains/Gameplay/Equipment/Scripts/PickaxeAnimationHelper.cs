using System.Collections;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace Domains.Gameplay.Equipment.Scripts
{
    public class PickaxeAnimationHelper : MonoBehaviour
    {
        [SerializeField] private MMFeedbacks pickaxeAnimationFeedbacks;
        [SerializeField] private Vector3 pickaxeBumpPosition;
        [SerializeField] private Vector3 moveToRotationValue;
        private MMSpringPosition springPosition;

        private MMSpringRotation springRotation;

        private void Awake()
        {
            springPosition = GetComponent<MMSpringPosition>();
            springRotation = GetComponent<MMSpringRotation>();
        }

        public void OnPickaxeUse()
        {
            if (springPosition != null) springPosition.Bump(pickaxeBumpPosition);
        }

        private IEnumerator PickaxeAnimation()
        {
            if (springRotation != null)
            {
                springRotation.MoveToAdditive(moveToRotationValue);
                yield return new WaitForEndOfFrame();
                springRotation.MoveToAdditive(-moveToRotationValue);
            }
        }
    }
}