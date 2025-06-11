using System;
using UnityEngine;

namespace Domains.Gameplay.Objects.Powerups
{
    [Serializable]
    public enum PowerUpType
    {
        SpeedBoost
    }

    [CreateAssetMenu(fileName = "PowerUp", menuName = "Scriptable Objects/Items/PowerUp")]
    public class PowerUpScriptableObject : ScriptableObject
    {
        public string powerUpId;
        public PowerUpType powerUpType;
        public float duration;
        public Sprite icon;
        public float multiplier = 1f;
    }
}