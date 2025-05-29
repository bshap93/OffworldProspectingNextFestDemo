using UnityEngine;

namespace Domains.Player.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "GameLevelStatProfile", menuName = "Scriptable Objects/GameLevelStatProfile")]
    public class GameLevelStatProfile : ScriptableObject
    {
        public int numBeacons;
        public int numHibernationPods;
        public bool skipTutorial;
    }
}