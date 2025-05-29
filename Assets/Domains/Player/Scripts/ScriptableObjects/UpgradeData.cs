using Domains.Player.Events;
using UnityEngine;

namespace Domains.Player.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "NewUpgrade", menuName = "Upgrades/UpgradeData")]
    public class UpgradeData : ScriptableObject
    {
        public string upgradeTypeName;
        public string[] upgradeNames;
        public string description;
        public int[] upgradeCosts;
        public Color[] upgradeColors;
        public Material[] upgradeMaterials;

        public AudioClip[] upgradeSounds;

        [Header("Upgrade Effects")] public UpgradeEffectType[] effectTypes;
        public float[] effectValues; // Only relevant for multipliers/additions
        public string[] toolChangeIDs; // Only relevant for tool changes (leave empty otherwise)

        [Header("Secondary Effects")] public UpgradeEffectType[] secondaryEffectTypes;
        public float[] secondaryEffectValues; // Only relevant for multipliers/additions
    }
}