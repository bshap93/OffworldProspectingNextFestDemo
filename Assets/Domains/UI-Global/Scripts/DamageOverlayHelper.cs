using System.Collections;
using Domains.Player.Events;
using Domains.Player.Scripts;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using UnityEngine;

namespace Domains.UI_Global.Scripts
{
    public class DamageOverlayHelper : MonoBehaviour, MMEventListener<HealthEvent>
    {
        public float targetAlpha = 0.3f;
        private MMSpringImageAlpha _damageOverlay;

        private void Start()
        {
            _damageOverlay = GetComponent<MMSpringImageAlpha>();
        }

        private void OnEnable()
        {
            this.MMEventStartListening();
        }

        private void OnDisable()
        {
            this.MMEventStopListening();
        }


        public void OnMMEvent(HealthEvent eventType)
        {
            if (eventType.EventType == HealthEventType.ConsumeHealth &&
                eventType.ByValue > PlayerHealthManager.MaxHealthPoints)
                StartCoroutine(AnimateDeathOverlay());

            if (eventType.EventType == HealthEventType.ConsumeHealth)
                StartCoroutine(AnimateDamageOverlay());
        }

        private IEnumerator AnimateDamageOverlay()
        {
            _damageOverlay.MoveToAdditive(targetAlpha);
            yield return new WaitForSeconds(0.5f);
            _damageOverlay.MoveToSubtractive(targetAlpha);
        }

        private IEnumerator AnimateDeathOverlay()
        {
            _damageOverlay.MoveToAdditive(targetAlpha);
            yield return new WaitForSeconds(2f);
            _damageOverlay.MoveToSubtractive(targetAlpha);
        }
    }
}