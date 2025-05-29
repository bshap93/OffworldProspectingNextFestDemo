using Domains.Player.Events;
using MoreMountains.Tools;
using UnityEngine;

namespace Domains.Scripts_that_Need_Sorting
{
    public class SpotlightController : MonoBehaviour, MMEventListener<PlayerPositionEvent>
    {
        public Light digSpotlight;
        public float spotlightStrengthenDepth = 2f;

        public float initialSpotlightAngle = 30f;
        public float initialSpotlightIntensity;

        public float increasedSpotlightAngle = 45f;
        public float increasedSpotlightIntensity = 1.5f;

        private float currentPlayerDepth;


        private void Update()
        {
            UpdateLight();
        }

        private void OnEnable()
        {
            this.MMEventStartListening();
        }

        private void OnDisable()
        {
            this.MMEventStopListening();
        }

        public void OnMMEvent(PlayerPositionEvent eventType)
        {
            if (eventType.EventType == PlayerPositionEventType.ReportDepth) currentPlayerDepth = eventType.Position.y;
        }

        private void UpdateLight()
        {
            if (digSpotlight != null)
            {
                if (Mathf.Abs(currentPlayerDepth) > spotlightStrengthenDepth)
                {
                    digSpotlight.spotAngle = increasedSpotlightAngle;
                    digSpotlight.intensity = increasedSpotlightIntensity;
                }
                else
                {
                    digSpotlight.spotAngle = initialSpotlightAngle;
                    digSpotlight.intensity = initialSpotlightIntensity;
                }
            }
        }
    }
}