using System.Collections;
using Domains.Gameplay.Equipment.Events;
using MoreMountains.Tools;
using UnityEngine;

public class ProgressBarBlue : MonoBehaviour, MMEventListener<EquipmentEvent>
{
    [SerializeField] protected MMProgressBar cooldownProgressBar;
    [SerializeField] protected CanvasGroup cooldownCanvasGroup;


    private void OnEnable()
    {
        this.MMEventStartListening();
    }

    private void OnDisable()
    {
        this.MMEventStopListening();
    }

    public void OnMMEvent(EquipmentEvent eventType)
    {
        ResetCooldownBar();
        HideCooldownBar();
    }


    // Catches Equipment Events and Disappears, Resets, or Equivalent
    public IEnumerator ShowCooldownBarCoroutine(float duration)
    {
        if (cooldownProgressBar == null) yield break;

        var elapsed = 0f;
        cooldownCanvasGroup.alpha = 1f;
        cooldownProgressBar.UpdateBar01(0f); // ← show full immediately


        while (elapsed < duration)
        {
            cooldownProgressBar.UpdateBar01(elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        cooldownProgressBar.UpdateBar01(1f);
        cooldownCanvasGroup.alpha = 0f;
    }

    public void HideCooldownBar()
    {
        if (cooldownCanvasGroup == null) return;
        cooldownCanvasGroup.alpha = 0f;
    }

    public void ResetCooldownBar()
    {
        if (cooldownProgressBar == null) return;
        cooldownProgressBar.UpdateBar01(0f);
    }
}