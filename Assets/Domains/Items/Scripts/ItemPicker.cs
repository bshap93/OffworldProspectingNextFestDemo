using System;
using System.Collections;
using Domains.Gameplay.Mining.Scripts;
using Domains.Input.Scripts;
using Domains.Player.Scripts;
using Domains.Scene.Scripts;
using Gameplay.Events;
using INab.Dissolve;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// Added for CustomInputBindings

namespace Domains.Items.Scripts
{
    public class ItemPicker : MonoBehaviour, IInteractable
    {
        [FormerlySerializedAs("UniqueID")] public string uniqueID;
        [FormerlySerializedAs("ItemType")] public BaseItem itemType;
        [FormerlySerializedAs("Quantity")] public int quantity = 1;

        [Header("Events")] public UnityEvent OnItemPicked;

        [Header("Interaction Settings")] [Tooltip("How long the interact key must be held to pick up the item")]
        public float interactionHoldTime = 1.0f; // Time required to hold the interact key

        [Header("Feedbacks")] [Tooltip("Feedbacks to play when the item is picked up")]
        public MMFeedbacks pickedMmFeedbacks; // Feedbacks to play when the item is picked up


        [FormerlySerializedAs("NotPickable")] public bool notPickable; // If true, the item cannot be picked up
        public GameObject interactablePrompt;

        [FormerlySerializedAs("_dissolver")] [SerializeField]
        private Dissolver dissolver;

        public bool IsObjective;
        private bool _interactionComplete;
#pragma warning disable CS0414 // Field is assigned but its value is never used
        private bool _isBeingDestroyed;
#pragma warning restore CS0414 // Field is assigned but its value is never used

#pragma warning disable CS0414 // Field is assigned but its value is never used
        private bool _isInRange;
#pragma warning restore CS0414 // Field is assigned but its value is never used

        // Track interaction state

        private Inventory.Inventory _targetInventory;

        private void Awake()
        {
            if (string.IsNullOrEmpty(uniqueID)) uniqueID = Guid.NewGuid().ToString(); // Generate only if unset
        }

        private void Start()
        {
            // Wait for the PickableManager to finish loading before checking if this item is picked
            StartCoroutine(InitializeAfterPickableManager());

            _targetInventory = FindFirstObjectByType<Inventory.Inventory>();

            if (_targetInventory == null) UnityEngine.Debug.LogWarning("No inventory found in scene");

            if (pickedMmFeedbacks != null) pickedMmFeedbacks.Initialization(gameObject);
        }


        private void OnDestroy()
        {
            _isBeingDestroyed = true;

            _isInRange = false;
            enabled = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player")) _isInRange = true;
        }

        private void OnTriggerExit(Collider exitCollider)
        {
            if (exitCollider.CompareTag("Player")) _isInRange = false;
        }

        public void Interact()
        {
            // If interaction key is still held
            if (CustomInputBindings.IsInteractPressed()) PickItem();
        }

        public void ShowInteractablePrompt()
        {
            if (interactablePrompt != null) interactablePrompt.SetActive(true);
        }

        public void HideInteractablePrompt()
        {
            if (interactablePrompt != null) interactablePrompt.SetActive(false);
        }

        public void TryCompleteObjective()
        {
            if (IsObjective)
            {
            }
        }

        private IEnumerator InitializeAfterPickableManager()
        {
            // Wait a frame to ensure PickableManager has initialized
            yield return null;

            // Now check if this item should be destroyed
            if (PickableManager.IsItemPicked(uniqueID)) Destroy(gameObject);
        }

        // In the PickItem method of ItemPicker.cs
        public void PickItem()
        {
            // Check if item is already in inventory
            if (PlayerInventoryManager.GetItem(uniqueID) != null)
            {
                UnityEngine.Debug.LogWarning("Item already in inventory! Skipping pickup.");
                return;
            }

            var entry = new Inventory.Inventory.InventoryEntry(uniqueID, itemType);
            if (PlayerInventoryManager.AddItem(entry))
            {
                // Play feedback
                pickedMmFeedbacks?.PlayFeedbacks();

                // Save item as picked
                PickableManager.AddPickedItem(uniqueID, true);

                // Trigger item picked event
                ItemEvent.Trigger(ItemEventType.Picked, entry, transform);

                OnItemPicked?.Invoke();

                StartCoroutine(DissolveThenDestroy());
            }
            else
            {
                UnityEngine.Debug.LogWarning("Inventory full! Cannot pick up item.");
            }
        }

        private IEnumerator DissolveThenDestroy()
        {
            // Check if dissolver is assigned
            if (dissolver == null)
            {
                UnityEngine.Debug.LogWarning("Dissolver not assigned! Destroying object without dissolve.");
                Destroy(gameObject);
                yield break;
            }

            // Start dissolving
            dissolver.Dissolve();
            yield return new WaitForSeconds(dissolver.duration);
            // Destroy game object
            Destroy(gameObject);
            dissolver.Materialize();
        }
    }
}