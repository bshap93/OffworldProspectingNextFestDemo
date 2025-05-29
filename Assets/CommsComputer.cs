using Domains.UI_Global.Events;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class CommsComputer : MonoBehaviour
{
    [FormerlySerializedAs("openCommsComputerFB")] [SerializeField] private MMFeedbacks openCommsComputerFb;

    [FormerlySerializedAs("_openCommsComputerEvent")] public UnityEvent openCommsComputerEvent;

    public void OpenCommsComputer()
    {
        UIEvent.Trigger(UIEventType.OpenCommsComputer);
        openCommsComputerFb?.PlayFeedbacks();
        openCommsComputerEvent?.Invoke();
    }
}