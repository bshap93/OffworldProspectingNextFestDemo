#if !DISABLESTEAMWORKS  && STEAMWORKSNET
using UnityEngine.Events;

namespace Heathen.SteamworksIntegration
{
    [System.Serializable]
    public class LobbyResponseEvent : UnityEvent<Steamworks.EChatRoomEnterResponse> { }

    [System.Serializable]
    public class EResultEvent : UnityEvent<Steamworks.EResult> { }
}
#endif