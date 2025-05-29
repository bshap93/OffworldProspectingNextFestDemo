using System;
using MoreMountains.Tools;

namespace Domains.Gameplay.Mining.Scripts
{
    [Serializable]
    public enum ScannerEventType
    {
        ScannedObject
    }

    public struct ScannerEvent
    {
        private static ScannerEvent _e;

        public ScannerEventType ScannerEventType;

        public string TargetId;


        public static void Trigger(ScannerEventType toolType, string targetId)
        {
            _e.ScannerEventType = toolType;
            _e.TargetId = targetId;
            MMEventManager.TriggerEvent(_e);
        }
    }
}