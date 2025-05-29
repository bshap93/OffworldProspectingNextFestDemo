using Domains.UI_Global.Events;
using UnityEngine;

public class JetPackFeedbackController : MonoBehaviour
{
    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float shakeMagnitude = 0.5f;

    [SerializeField] private ParticleSystem steamCloudyMedium;


    public void TriggerShake()
    {
        CameraEvent.Trigger(CameraEventType.CameraShake, shakeDuration, shakeMagnitude);
    }
}