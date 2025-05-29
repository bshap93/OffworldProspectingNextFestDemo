using UnityEngine;

namespace Domains.Player.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "NewScriptableObjectScript", menuName = "Scriptable Objects/NewScriptableObjectScript")]
    public class NewScriptableObjectScript : ScriptableObject
    {
        public bool playerExpendsNoStamina;
        public bool playerIsInvulnerable;
    }
}