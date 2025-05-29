using Domains.Player.Camera;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using UnityEngine;

public class CameraAnimationHelper : MonoBehaviour, MMEventListener<CameraEffectEvent>
{
    public MMFeedbacks cameraShakeFeedback;

    private void Start()
    {
        if (cameraShakeFeedback != null)
            cameraShakeFeedback.Initialization(gameObject);
    }


    private void OnEnable()
    {
        this.MMEventStartListening();
    }

    private void OnDisable()
    {
        this.MMEventStopListening();
    }

    public void OnMMEvent(CameraEffectEvent eventType)
    {
        if (eventType.EventType == CameraEffectEventType.ShakeCameraPosition)
        {
            Debug.Log("Shake camera: attempting feedback");
            if (cameraShakeFeedback == null)
            {
                Debug.LogWarning("cameraShakeFeedback is null!");
                return;
            }

            if (!cameraShakeFeedback.isActiveAndEnabled)
            {
                Debug.LogWarning("cameraShakeFeedback is not active!");
            }

            cameraShakeFeedback.PlayFeedbacks(transform.position);
        }
    }
}