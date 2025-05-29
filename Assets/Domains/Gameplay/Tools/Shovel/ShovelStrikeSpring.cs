using MoreMountains.Feedbacks;
using UnityEngine;

namespace Domains.Gameplay.Tools.Shovel
{
    public class ShovelStrikeSpring : MonoBehaviour
    {
        public MMSpringFloat rotationSpring;
        public Transform shovelHead;

        private Quaternion initialLocalRotation;

        private void Start()
        {
            initialLocalRotation = shovelHead.localRotation;

            rotationSpring = new MMSpringFloat
            {
                Damping = 0.3f,
                Frequency = 6f
            };
            rotationSpring.SetInitialValue(0f);
        }

        private void Update()
        {
            rotationSpring.UpdateSpringValue(Time.deltaTime);

            // Apply as X-axis rotation offset
            
        }

        public void TriggerSwing()
        {
            rotationSpring.Bump(20f); // Degrees of rotation — adjust to taste
        }
    }
}