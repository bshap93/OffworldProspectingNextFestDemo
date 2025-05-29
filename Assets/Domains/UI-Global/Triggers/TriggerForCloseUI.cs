using Domains.UI_Global.Events;
using UnityEngine;

namespace Domains.UI_Global.Triggers
{
    public class TriggerForCloseUI : MonoBehaviour
    {
        public void TriggerCloseVendorUI()
        {
            UIEvent.Trigger(UIEventType.CloseVendorConsole);
            UIEvent.Trigger(UIEventType.CloseFuelConsole);
        }

        public void TriggerCloseUI()
        {
            UIEvent.Trigger(UIEventType.CloseUI);
        }

        public void TriggerCloseCommsComputer()
        {
            UIEvent.Trigger(UIEventType.CloseCommsComputer);
        }

        public void TriggerCloseInfoPanel()
        {
            UIEvent.Trigger(UIEventType.CloseInfoPanel);
        }
    }
}