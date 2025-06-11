using System;
using System.Collections;
using Domains.Player.Events;
using Domains.Player.Progression;
using Domains.UI_Global.Events;
using INab.Dissolve;
using UnityEngine;

namespace Domains.Items.Scripts
{
    public class CaseInteractable : InteractableObjective
    {
        [SerializeField] private Dissolver dissolver;

        public override void Interact()
        {
            if (hasBeenInteractedWith) return;
            CurrencyEvent.Trigger(CurrencyEventType.AddCurrency, rewardAmount);
            AlertEvent.Trigger(AlertReason.CreditsAdded, rewardAmount! + " Credits Added to your account",
                "Beacon Interacted", null, null, Color.white);

            interactFeedbacks?.PlayFeedbacks();
            hasBeenInteractedWith = true;
            ProgressionManager.AddInteractableObjective(uniqueID, true);
            OnInteractableInteract?.Invoke();
            StartCoroutine(DissolveThenDestroy());
        }

        protected override IEnumerator InitializeAfterProgressionManager()
        {
            throw new NotImplementedException();
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
        }
    }
}