using System.Collections;
using DG.Tweening;
using Domains.UI_Global.Events;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraEffectHelper : MonoBehaviour, MMEventListener<CameraEvent>
{
    [FormerlySerializedAs("otherFeedbacks")] [SerializeField]
    private MMFeedbacks otherShakeRelatedFeedbacks;

    [SerializeField] private float duration = 0.3f;

    private DOTweenAnimation cameraShakeAnimation;
    private bool isStationary;

    private void Awake()
    {
        isStationary = true;
    }

    private void Start()
    {
        cameraShakeAnimation = GetComponent<DOTweenAnimation>();
        if (cameraShakeAnimation == null)
        {
            Debug.LogError("DOTweenAnimation component not found on this GameObject.");
            return;
        }

        cameraShakeAnimation.DOPause();
    }

    private void OnEnable()
    {
        this.MMEventStartListening();
    }

    private void OnDisable()
    {
        this.MMEventStopListening();
    }

    public void OnMMEvent(CameraEvent eventType)
    {
        switch (eventType.EventType)
        {
            case CameraEventType.CameraShake:
                StartShakeCamera();
                break;
        }
    }

    public void StartShakeCamera()
    {
        StartCoroutine(ShakeCamera());
    }

    public IEnumerator ShakeCamera()
    {
        if (isStationary)
        {
            cameraShakeAnimation.DORestart();
            isStationary = false;

            otherShakeRelatedFeedbacks?.PlayFeedbacks();

            cameraShakeAnimation.DOPlay();

            Debug.Log("ShakeCamera animation started");

            yield return new WaitForSeconds(duration);

            isStationary = true;
        }
    }
}