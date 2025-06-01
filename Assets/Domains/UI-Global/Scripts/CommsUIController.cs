using Domains.UI_Global.Events;
using Domains.UI_Global.Interface;
using UnityEngine.Events;

namespace Domains.UI_Global.Scripts
{
    public class CommsUIController : UIController
    {
        public UnityEvent openCommsComputerEvent;
        public UnityEvent skipMessageEvent;

        private bool _isOpened;

        public CommsUIController(bool isPaused) : base(isPaused)
        {
        }


        public override void OnMMEvent(UIEvent eventType)
        {
            if (eventType.EventType == UIEventType.OpenCommsComputer)
            {
                if (_isOpened)
                {
                    skipMessageEvent?.Invoke();
                    return;
                } // Prevent opening if already opened

                OpenUI();
                openCommsComputerEvent?.Invoke();
                _isOpened = true;
            }

            else if (eventType.EventType == UIEventType.CloseCommsComputer)
            {
                CloseUI();
                _isOpened = false;
            }
        }
    }
}