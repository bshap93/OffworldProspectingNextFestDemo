using System;
using MoreMountains.Tools;

namespace Domains.Input.Events
{
    [Serializable]
    public enum InputSettingsEventType
    {
        InvertYAxis,
        SetMouseSensitivity,
        ShowKeyboardControls
    }

    public struct InputSettingsEvent
    {
        private static InputSettingsEvent _e;

        public bool? BoolValue;
        public float? FloatValue;

        public InputSettingsEventType EventType;

        public static void Trigger(InputSettingsEventType eventType, bool? value = null, float? floatValue = null)
        {
            {
                _e.EventType = eventType;
                _e.BoolValue = value;
                _e.FloatValue = floatValue;
                MMEventManager.TriggerEvent(_e);
            }
        }
    }
}