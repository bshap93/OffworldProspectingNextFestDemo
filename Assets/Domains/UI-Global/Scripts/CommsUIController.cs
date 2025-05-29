using Domains.UI_Global.Events;
using Domains.UI_Global.Interface;

namespace Domains.UI_Global.Scripts
{
    public class CommsUIController : UIController
    {
        public CommsUIController(bool isPaused) : base(isPaused)
        {
        }


        public override void OnMMEvent(UIEvent eventType)
        {
            if (eventType.EventType == UIEventType.OpenCommsComputer)
                OpenUI();
            else if (eventType.EventType == UIEventType.CloseCommsComputer) CloseUI();
        }
    }
}