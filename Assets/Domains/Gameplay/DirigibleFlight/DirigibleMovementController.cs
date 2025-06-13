using System;
using UnityEngine;

namespace Domains.Gameplay.DirigibleFlight
{
    [RequireComponent(typeof(Rigidbody))]
    public class DirigibleMovementController : MonoBehaviour
    {
        [Header("Input Values (Set by DirigibleInput)")]
        public float turnValue;

        public float thrustValue;
        public float changeHeightValue;

        [Header("Movement Settings")] [SerializeField]
        private float maxForwardThrust = 1500f;

        [SerializeField] private float maxBackwardThrust = 600f; // Less powerful in reverse
        [SerializeField] private float turnTorque = 800f;
        [SerializeField] private float stabilizationForce = 100f;

        [Header("Vertical Movement")] [SerializeField]
        private float verticalThrustForce = 1200f;

        [SerializeField] private float hoverStabilization = 50f;
        [SerializeField] private float maxTiltAngle = 15f; // Max forward/back tilt for quadcopter props

        [Header("Physics Settings")] [SerializeField]
        private float dragCoefficient = 0.98f; // Atmospheric drag in thin air

        [SerializeField] private float angularDragCoefficient = 0.95f;
        [SerializeField] private float gravityMultiplier = 0.7f; // Reduced gravity on colony planet
        [SerializeField] private float buoyancyForce = 800f; // Gas bag lift

        [Header("Responsiveness")] [SerializeField]
        private float thrustResponseTime = 0.3f;

        [SerializeField] private float turnResponseTime = 0.25f;
        [SerializeField] private float verticalResponseTime = 0.4f;

        // Quadcopter tilt simulation
        private float currentQuadTiltAngle;
        private float currentThrust;
        private float currentTurnInput;
        private float currentVerticalInput;

        // Private variables
        private Rigidbody rb;
        private Vector3 windForce = Vector3.zero; // For future wind implementation

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();

            // Set up rigidbody for dirigible physics
            rb.mass = 2000f; // Heavy but not too heavy
            rb.linearDamping = 0.5f; // Base drag
            rb.angularDamping = 2f; // Resistance to rotation
            rb.useGravity = true;
        }

        private void Start()
        {
            // Apply initial buoyancy to counteract some gravity
            rb.AddForce(Vector3.up * buoyancyForce, ForceMode.Force);
        }

        private void FixedUpdate()
        {
            HandleMovement();
            HandleTurning();
            HandleVerticalMovement();
            ApplyDrag();
            ApplyBuoyancy();
            ApplyStabilization();
        }

        private void HandleMovement()
        {
            // Smooth thrust input with response time
            var targetThrust = thrustValue * (thrustValue >= 0 ? maxForwardThrust : maxBackwardThrust);
            currentThrust = Mathf.Lerp(currentThrust, targetThrust, Time.fixedDeltaTime / thrustResponseTime);

            // Apply thrust in forward direction
            var thrustForce = transform.forward * currentThrust;
            rb.AddForce(thrustForce, ForceMode.Force);

            // Reduce effectiveness at high speeds (realistic air resistance)
            var speedFactor = Mathf.Clamp01(1f - rb.linearVelocity.magnitude / 30f);
            rb.AddForce(thrustForce * (speedFactor * 0.5f), ForceMode.Force);
        }

        private void HandleTurning()
        {
            // Smooth turn input
            currentTurnInput = Mathf.Lerp(currentTurnInput, turnValue, Time.fixedDeltaTime / turnResponseTime);

            // Apply turning torque around Y-axis
            var torque = Vector3.up * (currentTurnInput * turnTorque);
            rb.AddTorque(torque, ForceMode.Force);

            // Turning is more effective when moving forward (like a real aircraft)
            var speedFactor = Mathf.Clamp01(rb.linearVelocity.magnitude / 10f);
            var speedBasedTorque = Vector3.up * (currentTurnInput * turnTorque * speedFactor * 0.3f);
            rb.AddTorque(speedBasedTorque, ForceMode.Force);
        }

        private void HandleVerticalMovement()
        {
            // Smooth vertical input
            currentVerticalInput = Mathf.Lerp(currentVerticalInput, changeHeightValue,
                Time.fixedDeltaTime / verticalResponseTime);

            // Calculate quadcopter tilt based on vertical input and forward movement
            var targetTiltAngle = -thrustValue * maxTiltAngle * 0.5f; // Tilt forward when thrusting
            currentQuadTiltAngle = Mathf.Lerp(currentQuadTiltAngle, targetTiltAngle, Time.fixedDeltaTime * 2f);

            // Base vertical thrust from quadcopters
            var verticalForce = Vector3.up * (currentVerticalInput * verticalThrustForce);

            // Reduce vertical thrust effectiveness when tilted (realistic physics)
            var tiltEfficiency = Mathf.Cos(currentQuadTiltAngle * Mathf.Deg2Rad);
            verticalForce *= tiltEfficiency;

            // Add slight forward component when tilted
            if (Mathf.Abs(currentQuadTiltAngle) > 1f)
            {
                var tiltThrust = transform.forward *
                                 (Mathf.Sin(currentQuadTiltAngle * Mathf.Deg2Rad) * verticalThrustForce * 0.3f);
                rb.AddForce(tiltThrust, ForceMode.Force);
            }

            rb.AddForce(verticalForce, ForceMode.Force);
        }

        private void ApplyDrag()
        {
            // Apply atmospheric drag (reduced due to thin air)
            var dragForce = -rb.linearVelocity * (rb.linearVelocity.magnitude * dragCoefficient * 0.1f);
            rb.AddForce(dragForce, ForceMode.Force);

            // Apply angular drag
            var angularDragTorque = -rb.angularVelocity * (rb.angularVelocity.magnitude * angularDragCoefficient * 10f);
            rb.AddTorque(angularDragTorque, ForceMode.Force);
        }

        private void ApplyBuoyancy()
        {
            // Constant buoyancy from gas bags (counters gravity partially)
            var adjustedGravity = Physics.gravity.y * gravityMultiplier;
            var buoyancyVector = Vector3.up * (buoyancyForce + Mathf.Abs(adjustedGravity * rb.mass * 0.8f));
            rb.AddForce(buoyancyVector, ForceMode.Force);
        }

        private void ApplyStabilization()
        {
            // Horizontal stabilization - resist unwanted lateral movement
            var lateralVelocity = Vector3.Project(rb.linearVelocity, transform.right);
            var stabilizationForceVector = -lateralVelocity * stabilizationForce;
            rb.AddForce(stabilizationForceVector, ForceMode.Force);

            // Rotational stabilization - resist unwanted rolling and pitching
            var unwantedAngularVel = new Vector3(rb.angularVelocity.x, 0, rb.angularVelocity.z);
            var stabilizationTorque = -unwantedAngularVel * hoverStabilization;
            rb.AddTorque(stabilizationTorque, ForceMode.Force);
        }

        // Method for future wind implementation
        public void ApplyWind(Vector3 windDirection, float windStrength)
        {
            windForce = windDirection.normalized * windStrength;
            rb.AddForce(windForce, ForceMode.Force);
        }

        // Public method to get current status (useful for UI or debugging)
        public DirigibleStatus GetStatus()
        {
            return new DirigibleStatus
            {
                velocity = rb.linearVelocity,
                angularVelocity = rb.angularVelocity,
                currentThrust = currentThrust,
                quadTiltAngle = currentQuadTiltAngle,
                altitude = transform.position.y
            };
        }
    }

    // Data structure for dirigible status
    [Serializable]
    public struct DirigibleStatus
    {
        public Vector3 velocity;
        public Vector3 angularVelocity;
        public float currentThrust;
        public float quadTiltAngle;
        public float altitude;
    }
}