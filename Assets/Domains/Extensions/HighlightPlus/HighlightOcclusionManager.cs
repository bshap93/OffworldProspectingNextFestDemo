using HighlightPlus;
using UnityEngine;

namespace Domains.Extensions.HighlightPlus
{
    /// <summary>
    ///     Manages occlusion for HighlightTrigger components without modifying them
    /// </summary>
    public class HighlightOcclusionManager : MonoBehaviour
    {
        [Tooltip("Camera used for raycasting")]
        public Camera raycastCamera;

        [Tooltip("Layers that should block highlighting (e.g., terrain)")]
        public LayerMask terrainOcclusionMask = 0;

        [Tooltip("Maximum distance for occlusion check")]
        public float maxOcclusionDistance = 100f;

        [Tooltip("How often to perform occlusion checks (seconds)")]
        public float updateInterval = 0.1f;

        private float timer;

        private void Start()
        {
            if (raycastCamera == null)
            {
                raycastCamera = Camera.main;
                if (raycastCamera == null)
                    UnityEngine.Debug.LogError("HighlightOcclusionManager: No camera assigned or found!");
            }
        }

        private void Update()
        {
            timer += Time.deltaTime;
            if (timer >= updateInterval)
            {
                timer = 0f;
                CheckOcclusion();
            }
        }

        private void CheckOcclusion()
        {
            if (raycastCamera == null || terrainOcclusionMask == 0)
                return;

            // Get all highlighted objects
            var highlightEffects = FindObjectsByType<HighlightEffect>(FindObjectsSortMode.None);
            foreach (var effect in highlightEffects)
            {
                if (!effect.highlighted)
                    continue;

                // Get the position to check (center of renderer bounds if possible)
                Vector3 objectPosition;
                var renderer = effect.GetComponent<Renderer>();
                if (renderer != null)
                    objectPosition = renderer.bounds.center;
                else
                    objectPosition = effect.transform.position;

                // Direction from camera to object
                var direction = objectPosition - raycastCamera.transform.position;
                var distance = direction.magnitude;

                // Check if terrain is blocking the view
                if (Physics.Raycast(raycastCamera.transform.position, direction.normalized, out var hit, distance,
                        terrainOcclusionMask))
                    // Terrain is blocking the view to this object
                    if (hit.distance < distance - 0.1f) // Small threshold to prevent z-fighting issues
                        // Disable highlighting
                        effect.SetHighlighted(false);
            }
        }
    }
}