using System.Collections;
using Domains.Input.Scripts;
using Domains.Player.Scripts;
using Domains.UI_Global.Events;
using Lightbug.CharacterControllerPro.Core;
using MoreMountains.Tools;
using UnityEngine;

namespace Domains.Scripts_that_Need_Sorting
{
    public class SpawnManager : MonoBehaviour, MMEventListener<PlayerStatusEvent>
    {
        public static SpawnManager Instance;
        public GameObject player01;

        // In SpawnManager.cs
        [SerializeField] private PlayerDeathManager playerDeathManager; // Reference to death manager
        private readonly float requiredHoldDuration = 1f; // Change to desired duration in seconds
        private TeleportPlayer _player01TeleportPlayer;
        private CharacterActor characterActor;

        private float eKeyHoldTime;

        private void Awake()
        {
            Instance = this;
            _player01TeleportPlayer = GetComponent<TeleportPlayer>();

            characterActor = player01.GetComponent<CharacterActor>();

            playerDeathManager = FindFirstObjectByType<PlayerDeathManager>();

            if (characterActor == null)
                UnityEngine.Debug.LogError("CharacterActor component not found on player01.");

            if (_player01TeleportPlayer == null)
                UnityEngine.Debug.LogError("PositionAndRotationModifier component not found on SpawnManager.");
        }


        private void Update()
        {
            if (CustomInputBindings.IsResetHeld())
            {
                eKeyHoldTime += Time.deltaTime;

                if (eKeyHoldTime >= requiredHoldDuration)
                {
                    UnityEngine.Debug.Log("E key held long enough!");
                    StartCoroutine(ResetManually(0f))
                        ;

                    // Optional: Reset to avoid retriggering
                    eKeyHoldTime = 0f;
                }
            }
            else
            {
                // Reset if key is released
                eKeyHoldTime = 0f;
            }
        }

        private void OnEnable()

        {
            this.MMEventStartListening();
        }

        private void OnDisable()
        {
            this.MMEventStopListening();
        }


        public void OnMMEvent(PlayerStatusEvent eventType)
        {
            if (eventType.EventType == PlayerStatusEventType.Died)
                StartCoroutine(ResetManually(2f));

            if (eventType.EventType == PlayerStatusEventType.SoftReset
               )
                StartCoroutine(ResetManually(0f));
            if (eventType.EventType == PlayerStatusEventType.ResetManaully)
                StartCoroutine(ResetManually(0f));
        }

        private IEnumerator ResetManually(float delay)
        {
            if (delay > 0f)
                // Optional: Play a feedback or sound here
                playerDeathManager.manualResetDeathFeedbacks?.PlayFeedbacks();

            yield return new WaitForSeconds(delay);
            // Trigger your logic here
            playerDeathManager.SetPostFuelOutStats();
            TeleportPlayerToSpawn();

            AlertEvent.Trigger(AlertReason.ResetManually,
                "You were charged " + playerDeathManager.GetRescueExpense() + " credits for your rescue.",
                "Reset Manually");
        }


        private IEnumerator VerifyFuelAfterTeleport()
        {
            yield return new WaitForSeconds(0.2f);

            // Double-check that player has enough fuel to move
            if (PlayerFuelManager.FuelPoints < 10f)
            {
                UnityEngine.Debug.LogWarning("Player fuel still too low after teleport, forcing minimum");
                PlayerFuelManager.EnsureMinimumFuel();
            }
        }

// In SpawnManager.cs
        private void TeleportPlayerToSpawn()
        {
            _player01TeleportPlayer.Teleport(characterActor);

            // Debug when teleporting
            UnityEngine.Debug.Log("Player teleported to spawn point");
        }
    }
}