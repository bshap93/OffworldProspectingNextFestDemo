using UnityEngine;
using UnityEngine.Events;

public class TriggerColliderHandler : MonoBehaviour
{
    public enum SingleUseMode
    {
        OnEnter,
        OnExit,
        OnBoth
    }

    [SerializeField] private bool isActive = true;
    public UnityEvent onTriggerEnter;
    public UnityEvent onTriggerExit;

    [Header("Single Use Settings")] public bool singleUse;

    public SingleUseMode singleUseMode = SingleUseMode.OnEnter;

    private bool hasBeenUsed;

    private void OnTriggerEnter(Collider other)
    {
        if (!isActive || (singleUse && hasBeenUsed)) return;

        if (other.CompareTag("Player"))
        {
            onTriggerEnter?.Invoke();

            if (singleUse && (singleUseMode == SingleUseMode.OnEnter || singleUseMode == SingleUseMode.OnBoth))
                DeactivateTrigger();
        }
    }

    private void DeactivateTrigger()
    {
        hasBeenUsed = true;
        isActive = false;
        // Optionally disable the GameObject or just the component
        enabled = false;
    }
}