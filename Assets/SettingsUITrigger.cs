using Domains.UI_Global.Events;
using UnityEngine;

public class SettingsUITrigger : MonoBehaviour
{
    public void TriggerSettingsUI()
    {
        UIEvent.Trigger(UIEventType.OpenSettings);
    }
}