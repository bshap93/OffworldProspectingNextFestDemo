using Domains.Player.Events;
using Domains.Player.Scripts;
using UnityEngine;

namespace Domains.Scripts_that_Need_Sorting
{
    public class JetPackBehavior : MonoBehaviour
    {
        private readonly float jetpackCooldown = 0.36f;
        private float jetPackButtonHoldTime;
        private float nextJetpackActionTime;
        private Vector3 smoothDampVelocity = Vector3.zero;

        public void TryTriggerJetPackEffect()
        {
            if (Time.time >= nextJetpackActionTime)
            {
                JetPackBehaviorMethod(); // formerly only called by MMF Player
                nextJetpackActionTime = Time.time + jetpackCooldown;
            }
        }


        public void UpdateHoldTime(float deltaTime)
        {
            jetPackButtonHoldTime += deltaTime;
        }

        public void ResetHoldTime()
        {
            jetPackButtonHoldTime = 0f;
        }

        public float GetHoldTime()
        {
            return jetPackButtonHoldTime;
        }


        public void JetPackBehaviorMethod()
        {
            FuelEvent.Trigger(FuelEventType.ConsumeFuel, 5f, PlayerFuelManager.MaxFuelPoints);
        }

        public Vector3 ApplyJetpackLift(Vector3 currentVelocity, Vector3 upDirection, float targetHeight,
            float duration, float speedMultiplier)
        {
            var lift = upDirection * targetHeight * speedMultiplier;
            return Vector3.Lerp(currentVelocity, lift, 0.5f); // Blends in smoothly, but not fully SmoothDamp
        }
    }
}