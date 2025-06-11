using System.Collections;
using Domains.Input.Scripts;
using Domains.Player.Events;
using Domains.Player.Progression;
using Domains.UI_Global.Events;
using INab.Dissolve;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

namespace Domains.Items.Scripts
{
    public class BeaconInteractable : InteractableObjective
    {
        [SerializeField] private Dissolver dissolver;

        [FormerlySerializedAs("IsObjective")] public bool isObjective;

        [SerializeField] [CanBeNull] private GameObject parentObject;

        private InfoPanelActivator _infoPanelActivator;


        protected override void Start()
        {
            base.Start();
            StartCoroutine(InitializeAfterProgressionManager());

            _infoPanelActivator = GetComponent<InfoPanelActivator>();
            if (interactFeedbacks != null) interactFeedbacks.Initialization();
        }


        protected override IEnumerator InitializeAfterProgressionManager()
        {
            yield return null;


            if (ProgressionManager.IsObjectiveCollected(uniqueID))
            {
                if (parentObject != null)
                    Destroy(parentObject);
                else
                    Destroy(gameObject);
            }
        }

        public override void Interact()
        {
            if (hasBeenInteractedWith) return;
            if (_infoPanelActivator != null) _infoPanelActivator.ToggleInfoPanel();
            // FlagBeaconForTeleport();
        }

        public void FlagBeaconForTeleport()
        {
            CurrencyEvent.Trigger(CurrencyEventType.AddCurrency, rewardAmount);
            AlertEvent.Trigger(AlertReason.CreditsAdded, rewardAmount! + " Credits Added to your account",
                "Beacon Interacted", null, null, Color.white);

            interactFeedbacks?.PlayFeedbacks();
            hasBeenInteractedWith = true;
            ProgressionManager.AddInteractableObjective(uniqueID, true);
            OnInteractableInteract?.Invoke();
            StartCoroutine(DissolveThenDestroy());
        }

        public void TryCompleteObjective()
        {
            if (isObjective)
            {
            }
        }

        private IEnumerator DissolveThenDestroy()
        {
            // Check if dissolver is assigned
            if (dissolver == null)
            {
                UnityEngine.Debug.LogWarning("Dissolver not assigned! Destroying object without dissolve.");
                if (parentObject != null)
                    Destroy(parentObject);
                else
                    Destroy(gameObject);
                yield break;
            }

            // Start dissolving
            dissolver.Dissolve();
            yield return new WaitForSeconds(dissolver.duration);
            // Destroy game object
            if (parentObject != null)
                Destroy(parentObject);
            else
                Destroy(gameObject);
        }
    }
}