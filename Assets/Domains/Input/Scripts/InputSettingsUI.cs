using Domains.Input.Events;
using Michsky.UI.Shift;
using UnityEngine;

namespace Domains.Input.Scripts
{
    public class InputSettingsUI : MonoBehaviour
    {
        [SerializeField] private SwitchManager invertYAxisToggle;
        [SerializeField] private SliderManager mouseSensitivitySlider;
        [SerializeField] private SwitchManager showKeyboardControlsToggle;

        private void Awake()
        {
            if (InputSettings.Instance != null) invertYAxisToggle.isOn = InputSettings.Instance.InvertYAxis;
        }

        public void OnInvertYAxisChanged(bool value)
        {
            InputSettingsEvent.Trigger(InputSettingsEventType.InvertYAxis, value);
        }

        public void OnMouseSensitivityChanged()
        {
            var sensitivity = mouseSensitivitySlider.mainSlider.value;
            InputSettingsEvent.Trigger(InputSettingsEventType.SetMouseSensitivity, floatValue: sensitivity);
        }

        public void OnShowKeyboardControlsChanged(bool value)
        {
            InputSettingsEvent.Trigger(InputSettingsEventType.ShowKeyboardControls, value);
        }
    }
}