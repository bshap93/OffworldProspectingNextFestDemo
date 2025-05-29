using UnityEngine;

public class SteamLinkOpener : MonoBehaviour
{
    public void OpenSteamPage()
    {
        Application.OpenURL("https://store.steampowered.com/app/3621020/OffWorld_Prospecting/");
    }
}