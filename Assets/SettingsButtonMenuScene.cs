using Domains.UI_Global.Events;
using UnityEngine;

public class SettingsButtonMenuScene : MonoBehaviour
{
    public void TriggerOpenSettings()
    {
        UIEvent.Trigger(UIEventType.OpenSettings);
    }


    public void TriggerCloseSettings()
    {
        UIEvent.Trigger(UIEventType.CloseSettings);
    }
}