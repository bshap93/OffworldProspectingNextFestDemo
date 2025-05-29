using Domains.Player.Scripts.ScriptableObjects;
using UnityEngine;
using UnityEngine.Serialization;

namespace Domains.Player.Scripts
{
    public class PlayerInfoSheet : MonoBehaviour
    {
        public static int WeightLimit;

        [FormerlySerializedAs("InitialStats")] private CharacterStatProfile initialStats;
        private static PlayerInfoSheet Instance { get; set; }


        private void Awake()
        {
            initialStats = Resources.Load<CharacterStatProfile>(CharacterResourcePaths.CharacterStatProfileFilePath);
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            if (initialStats != null)
                WeightLimit = initialStats.InitialWeightLimit;
            else
                UnityEngine.Debug.LogError("CharacterStatProfile not set in PlayerInfoSheet");
        }
    }
}