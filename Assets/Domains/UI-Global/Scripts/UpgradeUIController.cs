using Domains.UI_Global.Events;
using Domains.UI_Global.Interface;
using UnityEngine.Events;

namespace Domains.UI_Global.Scripts
{
    public class UpgradeUIController : UIController
    {
        public UnityEvent OnOpenUI;
        public UnityEvent OnCloseUI;

        public UpgradeUIController(bool isPaused) : base(isPaused)
        {
        }


        public override void OnMMEvent(UIEvent eventType)
        {
            if (eventType.EventType == UIEventType.OpenVendorConsole)
            {
                OpenUI();
                OnOpenUI?.Invoke();
            }
            else if (eventType.EventType == UIEventType.CloseVendorConsole)
            {
                CloseUI();
                OnCloseUI?.Invoke();
            }
        }
    }
}