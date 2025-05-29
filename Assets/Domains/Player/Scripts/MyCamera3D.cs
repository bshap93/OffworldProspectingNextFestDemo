using DG.Tweening;
using Domains.Input.Scripts;
using Domains.UI_Global.Events;
using Lightbug.CharacterControllerPro.Core;
using Lightbug.CharacterControllerPro.Implementation;
using Lightbug.Utilities;
using MoreMountains.Tools;
using UnityEngine;

namespace Domains.Player.Scripts
{
    [AddComponentMenu("Character Controller Pro/Demo/Camera/Camera 3D")]
    [DefaultExecutionOrder(ExecutionOrder.CharacterGraphicsOrder + 100)] // <--- Do your job after everything else
    public class MyCamera3D : MonoBehaviour, MMEventListener<UIEvent>, MMEventListener<CameraEvent>
    {
        public enum CameraMode
        {
            FirstPerson,
            ThirdPerson
        }

        // Add these fields for camera shake
        [Header("Camera Shake")] [SerializeField]
        private bool enableShake = true;

        [SerializeField] private float shakeStrength = 0.5f;
        [SerializeField] private float shakeDuration = 0.5f;
        [SerializeField] private int shakeVibrato = 10;
        [SerializeField] private float shakeRandomness = 90f;

        [Header("Inputs")] [SerializeField] private InputHandlerSettings inputHandlerSettings = new();

        [SerializeField] private string axes = "Camera";

        [SerializeField] private string zoomAxis = "Camera Zoom";

        [Header("Target")]
        [Tooltip(
            "Select the graphics root object as your target, the one containing all the meshes, sprites, animated models, etc. \n\nImportant: This will be the considered as the actual target (visual element).")]
        [SerializeField]
        private Transform targetTransform;

        [SerializeField] private Vector3 offsetFromHead = Vector3.zero;

        [Tooltip("The interpolation speed used when the height of the character changes.")] [SerializeField]
        private float heightLerpSpeed = 10f;

        [Header("View")] public CameraMode cameraMode = CameraMode.ThirdPerson;

        [Header("First Person")] public bool hideBody = true;

        [SerializeField] private GameObject bodyObject;

        [Header("Yaw")] public bool updateYaw = true;

        public float yawSpeed = 180f;


        [Header("Pitch")] public bool updatePitch = true;

        [SerializeField] private float initialPitch = 45f;

        public float pitchSpeed = 180f;

        [Range(1f, 85f)] public float maxPitchAngle = 80f;

        [Range(1f, 85f)] public float minPitchAngle = 80f;


        [Header("Roll")] public bool updateRoll;


        [Header("Zoom (Third person)")] public bool updateZoom = true;

        [Min(0f)] [SerializeField] private float distanceToTarget = 5f;

        [Min(0f)] public float zoomInOutSpeed = 40f;

        [Min(0f)] public float zoomInOutLerpSpeed = 5f;

        [Min(0f)] public float minZoom = 2f;

        [Min(0.001f)] public float maxZoom = 12f;


        [Header("Collision")] public bool collisionDetection = true;

        public bool collisionAffectsZoom;
        public float detectionRadius = 0.5f;
        public LayerMask layerMask = 0;
        public bool considerKinematicRigidbodies = true;
        public bool considerDynamicRigidbodies = true;
        private readonly RaycastHit[] hitsBuffer = new RaycastHit[10];
        private readonly RaycastHit[] validHits = new RaycastHit[10];
        private Renderer[] bodyRenderers;

        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────


        private CharacterActor characterActor;
        private Vector3 characterPosition;
        private Rigidbody characterRigidbody;

        private float currentDistanceToTarget;
        private Tweener currentShake;
        private float deltaPitch;

        private float deltaYaw;
        private float deltaZoom;

        private Vector3 lerpedCharacterUp = Vector3.up;
        private float lerpedHeight;


        private Vector3 previousLerpedCharacterUp = Vector3.up;

        private Vector3 shakeOffset = Vector3.zero;
        private float smoothedDistanceToTarget;

        private Transform viewReference;

        private void Awake()
        {
            Initialize(targetTransform);
        }


        private void Start()
        {
            characterPosition = targetTransform.position;

            previousLerpedCharacterUp = targetTransform.up;
            lerpedCharacterUp = previousLerpedCharacterUp;


            currentDistanceToTarget = distanceToTarget;
            smoothedDistanceToTarget = currentDistanceToTarget;

            viewReference.rotation = targetTransform.rotation;
            viewReference.Rotate(Vector3.right, initialPitch);

            lerpedHeight = characterActor.BodySize.y;
        }


        private void Update()
        {
            // Check if game is paused
            if (Time.timeScale == 0)
                return; // Stop all camera movement while paused

            if (targetTransform == null)
            {
                enabled = false;
                return;
            }

            var cameraAxes = inputHandlerSettings.InputHandler.GetVector2(axes) *
                             InputSettings.Instance.MouseSensitivity;

            // Add this block to apply Y-axis inversion
            if (InputSettings.Instance.InvertYAxis) cameraAxes.y = -cameraAxes.y;

            if (updatePitch)
                deltaPitch = -cameraAxes.y;

            if (updateYaw)
                deltaYaw = cameraAxes.x;

            if (updateZoom)
                deltaZoom = -inputHandlerSettings.InputHandler.GetFloat(zoomAxis);

            var dt = Time.fixedDeltaTime;

            UpdateCamera(dt);
        }

        private void OnEnable()
        {
            this.MMEventStartListening<UIEvent>();
            this.MMEventStartListening<CameraEvent>();

            if (characterActor == null)
                return;

            characterActor.OnTeleport += OnTeleport;
        }

        private void OnDisable()
        {
            this.MMEventStopListening<UIEvent>();
            this.MMEventStopListening<CameraEvent>();
            if (characterActor == null)
                return;

            characterActor.OnTeleport -= OnTeleport;

            // Kill any active shake
            if (currentShake != null && currentShake.IsActive())
                currentShake.Kill();
        }


        private void OnValidate()
        {
            initialPitch = Mathf.Clamp(initialPitch, -minPitchAngle, maxPitchAngle);
        }

        public void OnMMEvent(CameraEvent eventType)
        {
            switch (eventType.EventType)
            {
                case CameraEventType.CameraShake:
                    ShakeCamera(eventType.Magnitude, eventType.Duration);
                    break;
            }
        }

        public void OnMMEvent(UIEvent eventType)
        {
            if (eventType.EventType == UIEventType.OpenVendorConsole ||
                eventType.EventType == UIEventType.OpenFuelConsole ||
                eventType.EventType == UIEventType.OpenInfoDump ||
                eventType.EventType == UIEventType.OpenQuestDialogue ||
                eventType.EventType == UIEventType.OpenBriefing ||
                eventType.EventType == UIEventType.OpenCommsComputer
                || eventType.EventType == UIEventType.OpenInfoPanel
               ) EnableCameraControl(false);
            else if (eventType.EventType == UIEventType.CloseVendorConsole ||
                     eventType.EventType == UIEventType.CloseFuelConsole ||
                     eventType.EventType == UIEventType.CloseInfoDump ||
                     eventType.EventType == UIEventType.CloseQuestDialogue
                     || eventType.EventType == UIEventType.CloseBriefing ||
                     eventType.EventType == UIEventType.CloseCommsComputer
                     || eventType.EventType == UIEventType.CloseInfoPanel
                    ) EnableCameraControl(true);
        }

        // Add this method to trigger shake
        public void ShakeCamera(float strength = -1, float duration = -1)
        {
            if (!enableShake)
                return;

            UnityEngine.Debug.Log("ShakeCamera triggered");

            // Use default values if parameters not specified
            var useStrength = strength > 0 ? strength : shakeStrength;
            var useDuration = duration > 0 ? duration : shakeDuration;

            // Kill any existing shake
            if (currentShake != null && currentShake.IsActive())
                currentShake.Kill();

            // Reset offset
            shakeOffset = Vector3.zero;

            // Create shake tween
            currentShake = DOTween.Shake(
                () => Vector3.zero,
                x => shakeOffset = x,
                useDuration,
                useStrength,
                shakeVibrato,
                shakeRandomness
            ).SetAutoKill(true);
        }


        public void ToggleCameraMode()
        {
            cameraMode = cameraMode == CameraMode.FirstPerson ? CameraMode.ThirdPerson : CameraMode.FirstPerson;
        }

        public bool Initialize(Transform targetTransform)
        {
            if (targetTransform == null)
                return false;

            characterActor = targetTransform.GetComponentInBranch<CharacterActor>();

            if (characterActor == null || !characterActor.isActiveAndEnabled)
            {
                UnityEngine.Debug.Log("The character actor component is null, or it is not active/enabled.");
                return false;
            }

            characterRigidbody = characterActor.GetComponent<Rigidbody>();

            inputHandlerSettings.Initialize(gameObject);

            var referenceObject = new GameObject("Camera reference");
            viewReference = referenceObject.transform;

            if (bodyObject != null)
                bodyRenderers = bodyObject.GetComponentsInChildren<Renderer>();

            return true;
        }


        private void OnTeleport(Vector3 position, Quaternion rotation)
        {
            viewReference.rotation = rotation;
            transform.rotation = viewReference.rotation;

            lerpedCharacterUp = characterActor.Up;
            previousLerpedCharacterUp = lerpedCharacterUp;
        }

        private void HandleBodyVisibility()
        {
            if (cameraMode == CameraMode.FirstPerson)
            {
                if (bodyRenderers != null)
                    for (var i = 0; i < bodyRenderers.Length; i++)
                        if (bodyRenderers[i].GetType().IsSubclassOf(typeof(SkinnedMeshRenderer)))
                        {
                            var skinnedMeshRenderer = (SkinnedMeshRenderer)bodyRenderers[i];
                            if (skinnedMeshRenderer != null)
                                skinnedMeshRenderer.forceRenderingOff = hideBody;
                        }
                        else
                        {
                            bodyRenderers[i].enabled = !hideBody;
                        }
            }
            else
            {
                if (bodyRenderers != null)
                    for (var i = 0; i < bodyRenderers.Length; i++)
                    {
                        if (bodyRenderers[i] == null)
                            continue;

                        if (bodyRenderers[i].GetType().IsSubclassOf(typeof(SkinnedMeshRenderer)))
                        {
                            var skinnedMeshRenderer = (SkinnedMeshRenderer)bodyRenderers[i];
                            if (skinnedMeshRenderer != null)
                                skinnedMeshRenderer.forceRenderingOff = false;
                        }
                        else
                        {
                            bodyRenderers[i].enabled = true;
                        }
                    }
            }
        }


        private void UpdateCamera(float dt)
        {
            // Body visibility ---------------------------------------------------------------------
            HandleBodyVisibility();

            // Rotation -----------------------------------------------------------------------------------------
            lerpedCharacterUp = targetTransform.up;

            // Rotate the reference based on the lerped character up vector 
            var deltaRotation = Quaternion.FromToRotation(previousLerpedCharacterUp, lerpedCharacterUp);
            previousLerpedCharacterUp = lerpedCharacterUp;

            viewReference.rotation = deltaRotation * viewReference.rotation;


            // Yaw rotation -----------------------------------------------------------------------------------------        
            viewReference.Rotate(lerpedCharacterUp, deltaYaw * yawSpeed * dt, Space.World);

            // Pitch rotation -----------------------------------------------------------------------------------------            

            var angleToUp = Vector3.Angle(viewReference.forward, lerpedCharacterUp);


            var minPitch = -angleToUp + (90f - minPitchAngle);
            var maxPitch = 180f - angleToUp - (90f - maxPitchAngle);

            var pitchAngle = Mathf.Clamp(deltaPitch * pitchSpeed * dt, minPitch, maxPitch);
            viewReference.Rotate(Vector3.right, pitchAngle);

            // Roll rotation -----------------------------------------------------------------------------------------    
            if (updateRoll)
                viewReference.up =
                    lerpedCharacterUp; //Quaternion.FromToRotation( viewReference.up , lerpedCharacterUp ) * viewReference.up;

            // Position of the target -----------------------------------------------------------------------
            characterPosition = targetTransform.position;

            lerpedHeight = Mathf.Lerp(lerpedHeight, characterActor.BodySize.y, heightLerpSpeed * dt);
            var targetPosition = characterPosition + targetTransform.up * lerpedHeight +
                                 targetTransform.TransformDirection(offsetFromHead);
            viewReference.position = targetPosition;

            var finalPosition = viewReference.position;

            // ------------------------------------------------------------------------------------------------------
            if (cameraMode == CameraMode.ThirdPerson)
            {
                currentDistanceToTarget += deltaZoom * zoomInOutSpeed * dt;
                currentDistanceToTarget = Mathf.Clamp(currentDistanceToTarget, minZoom, maxZoom);

                smoothedDistanceToTarget = Mathf.Lerp(smoothedDistanceToTarget, currentDistanceToTarget,
                    zoomInOutLerpSpeed * dt);
                var displacement = -viewReference.forward * smoothedDistanceToTarget;

                if (collisionDetection)
                {
                    var hit = DetectCollisions(ref displacement, targetPosition);

                    if (collisionAffectsZoom && hit)
                        currentDistanceToTarget = smoothedDistanceToTarget = displacement.magnitude;
                }

                finalPosition = targetPosition + displacement;
            }


            // At the end, after finalPosition is calculated:
            transform.position = finalPosition + shakeOffset;
            transform.rotation = viewReference.rotation;
        }


        private bool DetectCollisions(ref Vector3 displacement, Vector3 lookAtPosition)
        {
            var hits = Physics.SphereCastNonAlloc(
                lookAtPosition,
                detectionRadius,
                Vector3.Normalize(displacement),
                hitsBuffer,
                currentDistanceToTarget,
                layerMask,
                QueryTriggerInteraction.Ignore
            );

            // Order the results
            var validHitsNumber = 0;
            for (var i = 0; i < hits; i++)
            {
                var hitBuffer = hitsBuffer[i];

                var detectedRigidbody = hitBuffer.collider.attachedRigidbody;

                // Filter the results ---------------------------
                if (hitBuffer.distance == 0)
                    continue;

                if (detectedRigidbody != null)
                {
                    if (considerKinematicRigidbodies && !detectedRigidbody.isKinematic)
                        continue;

                    if (considerDynamicRigidbodies && detectedRigidbody.isKinematic)
                        continue;

                    if (detectedRigidbody == characterRigidbody)
                        continue;
                }

                //----------------------------------------------            
                validHits[validHitsNumber] = hitBuffer;
                validHitsNumber++;
            }

            if (validHitsNumber == 0)
                return false;


            var distance = Mathf.Infinity;
            for (var i = 0; i < validHitsNumber; i++)
            {
                var hitBuffer = validHits[i];

                if (hitBuffer.distance < distance)
                    distance = hitBuffer.distance;
            }

            displacement = CustomUtilities.Multiply(Vector3.Normalize(displacement), distance);


            return true;
        }

        public void EnableCameraControl(bool enable)
        {
            updateYaw = enable;
            updatePitch = enable;
            updateZoom = enable;

            if (!enable)
            {
                deltaYaw = 0f;
                deltaPitch = 0f;
                deltaZoom = 0f;
            }
        }
    }
}