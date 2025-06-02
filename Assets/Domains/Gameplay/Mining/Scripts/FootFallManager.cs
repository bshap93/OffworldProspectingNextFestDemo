using Domains.Player.Scripts;
using Lightbug.CharacterControllerPro.Core;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace Domains.Gameplay.Mining.Scripts
{
    internal class FootFallManager : MonoBehaviour
    {
        [Header("Footsteps")] [SerializeField] private MMFeedbacks terrainFootstepFeedbacks;
        [SerializeField] private MMFeedbacks rockFootstepFeedbacks;

        [SerializeField] private MMFeedbacks chunkFootstepFeedbacks;
        [SerializeField] private MMFeedbacks defaultFootstepFeedbacks;
        [SerializeField] private MMFeedbacks lavaFootstepFeedbacks;
        [SerializeField] private float baseStepInterval = 0.5f;

        [SerializeField] private PlayerInteraction playerInteraction;

        [SerializeField] private CharacterActor characterActor;

        private float _footstepInterval;

        private float _footstepTimer;

        private bool _wasMovingLastFrame;


        private void Awake()
        {
            if (characterActor == null)
                characterActor = FindFirstObjectByType<CharacterActor>();

            if (playerInteraction == null)
                playerInteraction = FindFirstObjectByType<PlayerInteraction>();
        }

        private void Update()
        {
            UpdateFootsteps(Time.deltaTime);
        }

        private void UpdateFootsteps(float dt)
        {
            var isMoving = characterActor.IsGrounded && characterActor.PlanarVelocity.magnitude > 0.01f;

            if (isMoving)
            {
                // Dynamically scale interval
                var speed = characterActor.PlanarVelocity.magnitude;
                _footstepInterval = baseStepInterval / Mathf.Max(speed, 0.1f);
                _footstepInterval = Mathf.Clamp(_footstepInterval, baseStepInterval * 0.7f, baseStepInterval * 1.3f);

                // Trigger first footstep as soon as movement begins
                if (!_wasMovingLastFrame)
                    _footstepTimer = _footstepInterval;

                // Run step timer
                if (_footstepTimer >= _footstepInterval)
                {
                    PlayFootfallFeedback();
                    _footstepTimer = 0f;
                }
                else
                    // ERROR - no dt in context
                {
                    _footstepTimer += dt;
                }
            }
            else
            {
                _footstepTimer = 0f;
            }

            _wasMovingLastFrame = isMoving;
        }


        private void PlayFootfallFeedback()
        {
            var textureIndex = playerInteraction?.GetGroundTextureIndex() ?? -1;


            switch (textureIndex)
            {
                case 11:
                case 1:
                case 13:
                case 14:
                    chunkFootstepFeedbacks?.PlayFeedbacks();
                    break;


                case 0:
                case 12:
                case 15:
                case 10:
                    terrainFootstepFeedbacks?.PlayFeedbacks();
                    break;

                case 2:
                case 3:
                case 4:
                case 6:
                case 7:
                case 8:
                case 9:
                case 5:
                    rockFootstepFeedbacks?.PlayFeedbacks();
                    break;


                default:
                    defaultFootstepFeedbacks?.PlayFeedbacks();
                    break;
            }
        }
    }
}