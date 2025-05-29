using Domains.UI_Global.Events;
using UnityEngine;

public class CloseButtonBriefing : MonoBehaviour
{
    public void TriggerCloseBriefing()
    {
        UIEvent.Trigger(UIEventType.CloseBriefing);
    }
}