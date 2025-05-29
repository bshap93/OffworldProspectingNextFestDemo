using System.Collections;
using Domains.Player.Events;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using UnityEngine;

namespace Domains.UI_Global.Scripts
{
    public class RestoreHealthOverlayHelper : MonoBehaviour, MMEventListener<HealthEvent>
    {
        public float targetAlpha = 0.3f;
        public float duration = 0.5f;
        private MMSpringImageAlpha _restoreHealthOvrelay;

        private void Start()
        {
            _restoreHealthOvrelay = GetComponent<MMSpringImageAlpha>();
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
            if (eventType.EventType == HealthEventType.RecoverHealth ||
                eventType.EventType == HealthEventType.FullyRecoverHealth)
                StartCoroutine(AnimateRecoverHealthOVerlay());
        }

        private IEnumerator AnimateRecoverHealthOVerlay()
        {
            _restoreHealthOvrelay.MoveToAdditive(targetAlpha);
            yield return new WaitForSeconds(duration);
            _restoreHealthOvrelay.MoveToSubtractive(targetAlpha);
        }
    }
}