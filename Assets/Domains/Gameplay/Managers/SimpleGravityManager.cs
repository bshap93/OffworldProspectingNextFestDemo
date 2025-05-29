using System.Collections.Generic;
using Domains.Items.Scripts;
using UnityEngine;

namespace Domains.Gameplay.Managers
{
    /// <summary>
    ///     Simple gravity manager that prevents objects falling into the void
    ///     Only enables gravity when object has ground below AND isn't blocked by terrain
    /// </summary>
    public class SimpleGravityManager : MonoBehaviour
    {
        [Tooltip("Distance to check around object for blocking surfaces")]
        public float blockingCheckDistance = 2f;

        [Header("Performance")] [Tooltip("Max objects to check per frame")]
        public int maxChecksPerFrame = 3;

        [Tooltip("Max distance to check for ground below")]
        public float maxGroundCheckDistance = 10f;

        [Header("Debug")] public bool showDebugRays;

        [Header("Detection")] [Tooltip("Layer mask for terrain and digger meshes")]
        public LayerMask terrainLayerMask = 1;

        private readonly List<SimpleGravityObject> allObjects = new();

        private readonly Queue<SimpleGravityObject> checkQueue = new();

        private readonly HashSet<SimpleGravityObject> priorityObjects = new();

        public static SimpleGravityManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            ProcessChecks();
        }

        private void ProcessChecks()
        {
            var checksThisFrame = 0;

            // Check priority objects first (near recent digs)
            var priorityList = new List<SimpleGravityObject>(priorityObjects);
            foreach (var obj in priorityList)
            {
                if (checksThisFrame >= maxChecksPerFrame) break;
                if (obj != null)
                {
                    CheckObject(obj);
                    checksThisFrame++;
                    priorityObjects.Remove(obj);
                }
            }

            // Check regular queue
            while (checkQueue.Count > 0 && checksThisFrame < maxChecksPerFrame)
            {
                var obj = checkQueue.Dequeue();
                if (obj != null)
                {
                    CheckObject(obj);
                    checksThisFrame++;
                }
            }

            // Refill queue if empty
            if (checkQueue.Count == 0)
                foreach (var obj in allObjects)
                    if (obj != null)
                        checkQueue.Enqueue(obj);
        }

        private void CheckObject(SimpleGravityObject obj)
        {
            if (obj == null || obj.gameObject == null) return;

            var pos = obj.transform.position;

            // Step 1: Check for ground below
            var hasGroundBelow = HasSafeGroundBelow(pos, obj);

            // Step 2: Check for blocking surfaces around object
            var isBlocked = HasBlockingSurfaces(pos, obj);

            // Enable gravity only if has ground AND not blocked
            var shouldHaveGravity = hasGroundBelow && !isBlocked;

            obj.SetGravityEnabled(shouldHaveGravity);
        }

        private bool HasSafeGroundBelow(Vector3 position, SimpleGravityObject obj)
        {
            var objCollider = obj.GetComponent<Collider>();
            var bottomY = objCollider.bounds.min.y;

            var rayOrigin = new Vector3(position.x, bottomY, position.z);


            // Single raycast downward
            if (Physics.Raycast(rayOrigin, Vector3.down, out var hit,
                    maxGroundCheckDistance, terrainLayerMask))
            {
                // Check if the ground surface is facing up toward the object
                var toObject = (position - hit.point).normalized;
                var dot = Vector3.Dot(hit.normal, toObject);

                if (showDebugRays)
                    UnityEngine.Debug.DrawRay(rayOrigin, Vector3.down * maxGroundCheckDistance, Color.red, 0.5f);

                // Normal pointing toward object = safe ground
                return dot > 0.1f;
            }

            return false; // No ground = not safe
        }


        private bool HasBlockingSurfaces(Vector3 position, SimpleGravityObject obj)
        {
            var objCollider = obj.GetComponent<Collider>();
            var bottomY = objCollider.bounds.min.y;

            var rayOrigin = new Vector3(position.x, bottomY, position.z);
            // var rayOrigin = new Vector3(position.x, position.y, position.z);

            // Check 4 cardinal directions around object for blocking surfaces
            Vector3[] directions =
            {
                Vector3.forward,
                Vector3.back,
                Vector3.left,
                Vector3.right
            };

            foreach (var direction in directions)
            {
                // Always draw debug ray regardless of hit
                if (showDebugRays)
                    UnityEngine.Debug.DrawRay(rayOrigin, direction * blockingCheckDistance, Color.blue, 0.5f);

                if (Physics.Raycast(rayOrigin, direction, out var hit,
                        blockingCheckDistance, terrainLayerMask))
                {
                    // Check if surface normal points away from object (blocking)
                    var toObject = (rayOrigin - hit.point).normalized;
                    var dot = Vector3.Dot(hit.normal, toObject);

                    // Normal pointing away from object = blocking surface
                    if (dot < -0.1f)
                    {
                        if (showDebugRays)
                            UnityEngine.Debug.DrawRay(rayOrigin, direction * blockingCheckDistance, Color.cyan, 0.5f);
                        return true; // Found blocking surface
                    }

                    // Hit something but it's not blocking (normal points toward object)
                    if (showDebugRays)
                        UnityEngine.Debug.DrawRay(rayOrigin, direction * blockingCheckDistance, Color.yellow, 0.5f);
                }
            }

            return false; // No blocking surfaces found
        }

        public void RegisterObject(SimpleGravityObject obj)
        {
            if (!allObjects.Contains(obj))
            {
                allObjects.Add(obj);
                priorityObjects.Add(obj); // Check immediately
            }
        }

        public void UnregisterObject(SimpleGravityObject obj)
        {
            allObjects.Remove(obj);
            priorityObjects.Remove(obj);
        }

        /// <summary>
        ///     Call this after digging to check nearby objects immediately
        /// </summary>
        public void CheckNearbyObjects(Vector3 digPosition, float radius = 5f)
        {
            foreach (var obj in allObjects)
                if (obj != null && Vector3.Distance(obj.transform.position, digPosition) <= radius)
                    priorityObjects.Add(obj);
        }
    }
}