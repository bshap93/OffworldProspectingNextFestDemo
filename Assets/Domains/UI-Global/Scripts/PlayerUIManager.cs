using Domains.Player.Progression;
using Domains.UI_Global.Events;
using MoreMountains.Tools;
using UnityEngine;

namespace Domains.UI_Global.Scripts
{
    [DefaultExecutionOrder(0)]
    public class PlayerUIManager : MonoBehaviour, MMEventListener<UIEvent>
    {
        public static PlayerUIManager Instance;


        public bool uiIsOpen;


        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            if (ProgressionManager.TutorialFinished)
            {
                uiIsOpen = true;
            }
            else
            {
                UIEvent.Trigger(UIEventType.CloseUI);
                uiIsOpen = false;
            }
        }

        private void OnEnable()
        {
            this.MMEventStartListening();
        }

        private void OnDisable()
        {
            this.MMEventStopListening();
        }

        public void OnMMEvent(UIEvent eventType)
        {
            switch (eventType.EventType)
            {
                case UIEventType.CloseUI:
                    uiIsOpen = false;
                    break;
                case UIEventType.OpenFuelConsole:
                    uiIsOpen = true;
                    break;
                case UIEventType.CloseFuelConsole:
                    uiIsOpen = false;
                    break;
                case UIEventType.OpenVendorConsole:
                    uiIsOpen = true;
                    break;
                case UIEventType.OpenQuestDialogue:
                    uiIsOpen = true;
                    break;
                case UIEventType.CloseQuestDialogue:
                    uiIsOpen = false;
                    break;
                case UIEventType.CloseVendorConsole:
                    uiIsOpen = false;
                    break;
                case UIEventType.OpenBriefing:
                    uiIsOpen = true;
                    break;
                case UIEventType.CloseBriefing:
                    uiIsOpen = false;
                    break;
                case UIEventType.OpenInfoPanel:
                    uiIsOpen = true;
                    break;
                case UIEventType.CloseInfoPanel:
                    uiIsOpen = false;
                    break;
                case UIEventType.OpenCommsComputer:
                    uiIsOpen = true;
                    break;
                case UIEventType.CloseCommsComputer:
                    uiIsOpen = false;
                    break;
                case UIEventType.OpenInfoDump:
                    uiIsOpen = true;
                    break;
                case UIEventType.CloseInfoDump:
                    uiIsOpen = false;
                    break;
                case UIEventType.OpenSettings:
                    uiIsOpen = true;
                    break;
                case UIEventType.CloseSettings:
                    uiIsOpen = false;
                    break;
            }
        }
    }
}