using Domains.Gameplay.Managers;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace Domains.Items.Scripts
{
    /// <summary>
    ///     Simple component for objects that need gravity management
    /// </summary>
    public class SimpleGravityObject : MonoBehaviour
    {
        [Header("Physics")] public Rigidbody targetRigidbody;

        public bool makeKinematicWhenDisabled = true;

        [Header("Effects")] public GameObject enabledEffect;

        [SerializeField] private MMFeedbacks enabledFeedbacks;


        private bool gravityEnabled;

        private void Awake()
        {
            if (targetRigidbody == null)
                targetRigidbody = GetComponent<Rigidbody>();


            // Start with gravity disabled
            SetGravityEnabled(false);
        }

        private void Start()
        {
            // Register with manager
            SimpleGravityManager.Instance?.RegisterObject(this);
        }

        private void OnDestroy()
        {
            // Unregister
            SimpleGravityManager.Instance?.UnregisterObject(this);
        }

        public void SetGravityEnabled(bool enabled)
        {
            if (enabled == gravityEnabled) return;

            gravityEnabled = enabled;

            if (targetRigidbody != null)
            {
                if (enabled)
                {
                    // Enable physics
                    targetRigidbody.useGravity = true;
                    targetRigidbody.isKinematic = false;

                    // Play effects
                    if (enabledEffect != null)
                        Instantiate(enabledEffect, transform.position, Quaternion.identity);

                    enabledFeedbacks?.PlayFeedbacks();
                }
                else
                {
                    // Disable physics
                    targetRigidbody.useGravity = false;
                    if (makeKinematicWhenDisabled)
                        targetRigidbody.isKinematic = true;

                    // Stop motion
                    if (!targetRigidbody.isKinematic)
                    {
                        targetRigidbody.linearVelocity = Vector3.zero;
                        targetRigidbody.angularVelocity = Vector3.zero;
                    }

                }
            }
        }

        public bool IsGravityEnabled()
        {
            return gravityEnabled;
        }
    }
}