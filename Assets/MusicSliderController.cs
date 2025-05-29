using Domains.UI_Global.Events;
using UnityEngine;
using UnityEngine.UI;

public class MusicSliderController : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;

    [SerializeField] private float volumeLevel = 0.5f; // Default volume level

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        musicSlider = GetComponent<Slider>();

        if (musicSlider == null) Debug.LogError("MusicSliderController: Slider component not found.");

        // Set the initial value of the slider
        musicSlider.value = volumeLevel;
        TriggerChangeVolume();
    }

    // Update is called once per frame
    public void TriggerChangeVolume()
    {
        var sliderValue = musicSlider.value;
        AudioEvent.Trigger(AudioEventType.ChangeVolume, sliderValue);
    }
}