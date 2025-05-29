using Domains.UI_Global.Events;
using Domains.UI_Global.Interface;

namespace Domains.UI_Global.Scripts
{
    public class UpgradeUIController : UIController
    {
        public UpgradeUIController(bool isPaused) : base(isPaused)
        {
        }


        public override void OnMMEvent(UIEvent eventType)
        {
            if (eventType.EventType == UIEventType.OpenVendorConsole)
                OpenUI();
            else if (eventType.EventType == UIEventType.CloseVendorConsole) CloseUI();
        }
    }
}