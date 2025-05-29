using Domains.Player.Scripts.ScriptableObjects;
using MoreMountains.Tools;
using UnityEngine;

namespace Domains.Player.Events
{
    public enum UpgradeEventType
    {
        UpgradePurchased,
        UpgradeFailed,
        ShovelMiningSizeSet,
        PickaxeMiningSizeSet
    }

    public enum UpgradeType
    {
        Shovel,
        Pickaxe,
        Endurance,
        Inventory,
        Jetpack,
        None
    }

    public enum UpgradeEffectType
    {
        Multiplier, // e.g., 2x stamina
        Addition, // e.g., +5 weight
        ToolChange,
        None
    }

    public struct UpgradeEvent
    {
        private static UpgradeEvent _e;

        public UpgradeEventType EventType;
        public UpgradeType UpgradeType;
        public UpgradeData UpgradeData;
        public int UpgradeLevel;

        public UpgradeEffectType EffectType;
        public float EffectValue; // Used for multipliers and additions
        public float EffectValue2; // Used for multipliers and additions
        public string ToolId; // Only used if EffectType == ToolChange
        public Material CurrentMaterial; // Used for material changes

        public static void Trigger(UpgradeType upgradeType, UpgradeEventType upgradeEventType,
            UpgradeData upgradeData, int upgradeLevel, UpgradeEffectType effectType, float effectValue = 1,
            string toolId = null, float effectValue2 = 1, Material currentMaterial = null)
        {
            _e.EventType = upgradeEventType;
            _e.UpgradeType = upgradeType;
            _e.UpgradeData = upgradeData;
            _e.UpgradeLevel = upgradeLevel;
            _e.EffectType = effectType;
            _e.EffectValue =
                effectType == UpgradeEffectType.ToolChange ? 0 : effectValue; // Ensure only relevant values are used
            _e.ToolId = effectType == UpgradeEffectType.ToolChange ? toolId : null;
            _e.CurrentMaterial = currentMaterial;

            MMEventManager.TriggerEvent(_e);
        }
    }
}