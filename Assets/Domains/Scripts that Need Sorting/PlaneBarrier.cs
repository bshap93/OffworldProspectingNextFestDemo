using System;
using System.Collections;
using Domains.Player.Scripts;
using MoreMountains.Feedbacks;
using UnityEngine;

public class PlaneBarrier : MonoBehaviour
{
    [Serializable]
    public enum PlaneBarrierPosition
    {
        Top,
        Bottom,
        Vertical
    }

    [Serializable]
    public enum PlaneBarrierType
    {
        Lava,
        InvisibleWall
    }

    [SerializeField] private MMFeedbacks playerHitPlaneBarrierFeedbacks;

    public PlaneBarrierPosition planeBarrierPosition;
    public PlaneBarrierType planeBarrierType;

    private BoxCollider _boxCollider;

    // Start is called before the first frame update
    private void Start()
    {
        _boxCollider = GetComponent<BoxCollider>();
        _boxCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            if (planeBarrierType == PlaneBarrierType.Lava)
                StartCoroutine(DieByLava(other));
    }

    private IEnumerator DieByLava(Collider other)
    {
        playerHitPlaneBarrierFeedbacks?.PlayFeedbacks();
        yield return new WaitForSeconds(0.5f);
        PlayerStatusEvent.Trigger(PlayerStatusEventType.ResetManaully);
    }
}