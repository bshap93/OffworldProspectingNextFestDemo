using MoreMountains.Tools;

namespace Domains.UI_Global.Events
{
    public enum UIEventType
    {
        OpenVendorConsole,
        CloseVendorConsole,
        OpenFuelConsole,
        CloseFuelConsole,
        UpdateFuelConsole,
        CloseUI,
        OpenQuestDialogue,
        CloseQuestDialogue,
        OpenInfoDump,
        CloseInfoDump,
        OpenBriefing,
        CloseBriefing,
        OpenInfoPanel,
        CloseInfoPanel,
        OpenCommsComputer,
        CloseCommsComputer,
        CloseSettings,
        OpenSettings
    }

    public struct UIEvent
    {
        private static UIEvent _e;

        public UIEventType EventType;

        public int Index;

        public static void Trigger(UIEventType eventType, int index = 0)
        {
            _e.EventType = eventType;
            _e.Index = index;

            MMEventManager.TriggerEvent(_e);
        }
    }
}