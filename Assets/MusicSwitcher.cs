using Domains.UI_Global.Events;
using UnityEngine;

public class MusicSwitcher : MonoBehaviour
{
    public int clipIndex;
    public int[] customSequence;
    public bool loop = true; // Looping behavior
    public AudioEventType eventType;
    private readonly float _value = 1f; // Volume or other value

    public void TriggerAudioEvent()
    {
        AudioEvent.Trigger(eventType, _value, clipIndex, loop);
    }
}