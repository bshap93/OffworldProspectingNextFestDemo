using UnityEngine;

namespace Domains.Items.Scripts.ItemTypes
{
    [CreateAssetMenu(fileName = "Keycard", menuName = "Scriptable Objects/Items/Keycard")]
    public class Keycard : QuestItem
    {
        public string doorId;
    }
}