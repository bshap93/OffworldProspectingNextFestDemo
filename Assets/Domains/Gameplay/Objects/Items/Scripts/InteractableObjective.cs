using System;
using System.Collections;
using CompassNavigatorPro;
using Domains.Gameplay.Mining.Scripts;
using Domains.Gameplay.Objectives.Events;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Domains.Items.Scripts
{
    public abstract class InteractableObjective : MonoBehaviour, IInteractable
    {
        [FormerlySerializedAs("UniqueID")] public string uniqueID;

        [FormerlySerializedAs("RewardAmount")] public int rewardAmount;

        [SerializeField] protected bool hasBeenInteractedWith;
        [Header("Events")] public UnityEvent OnInteractableInteract;
        [SerializeField] ObjectiveEvent questControlEvent;
 


        public MMFeedbacks interactFeedbacks;


        protected CompassPro compassPro;

        protected CompassProPOI compassProPOI;

        protected bool InteractionComplete;

        protected void Awake()
        {
            if (string.IsNullOrEmpty(uniqueID)) uniqueID = Guid.NewGuid().ToString(); // Generate only if unset
        }

        protected virtual void Start()
        {
            compassProPOI = GetComponent<CompassProPOI>();
            if (compassProPOI != null) compassProPOI.ToggleIndicatorVisibility(false);


            compassPro = FindFirstObjectByType<CompassPro>();
            if (compassPro != null) compassPro.UpdateSettings();
        }


        protected void OnDestroy()
        {
            enabled = false;
        }

        public abstract void Interact();


        public void ShowInteractablePrompt()
        {
        }

        public void HideInteractablePrompt()
        {
            if (compassProPOI != null) compassProPOI.ToggleIndicatorVisibility(false);
        }

        protected abstract IEnumerator InitializeAfterProgressionManager();
    }
}