using Lightbug.CharacterControllerPro.Core;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace Domains.Gameplay.Tools
{
    public class ToolBob : MonoBehaviour
    {
        public float swayAmount = 0.01f;
        public float swaySpeed = 6f;
        public float bumpStrength = 0.015f;

        public CharacterActor character;

        private MMSpringFloat bobSpring;
        private Vector3 initialLocalPosition;

        private void Start()
        {
            initialLocalPosition = transform.localPosition;

            if (character == null)
                character = FindFirstObjectByType<CharacterActor>();

            // Set up spring
            bobSpring = new MMSpringFloat
            {
                Damping = 0.4f,
                Frequency = 5f
            };
            bobSpring.SetInitialValue(0f);
        }

        private void Update()
        {
            if (character == null) return;

            var velocity = character.PlanarVelocity.magnitude;

            // Add bump (optional: replace with footstep trigger)
            if (velocity > 0.1f && Mathf.FloorToInt(Time.time * swaySpeed) % 2 == 0) bobSpring.Bump(bumpStrength);

            bobSpring.UpdateSpringValue(Time.deltaTime);

            // Only sway when moving
            var sway = velocity > 0.1f ? Mathf.Sin(Time.time * swaySpeed) * swayAmount : 0f;

            var offset = new Vector3(0f, sway + bobSpring.CurrentValue, 0f);
            transform.localPosition = initialLocalPosition + offset;
        }
    }
}