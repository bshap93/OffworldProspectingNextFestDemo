using CompassNavigatorPro;
using UnityEngine;

namespace Domains.Player.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "ScannerProfileList", menuName = "Upgrades/Scanner Profile List")]
    public class ScannerProfileList : ScriptableObject
    {
        [Header("Scanner Profiles")] public ScanProfile[] scannerProfiles;
    }
}