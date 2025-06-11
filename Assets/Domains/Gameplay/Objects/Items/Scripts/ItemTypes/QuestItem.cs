using UnityEngine;

namespace Domains.Items.Scripts
{
    [CreateAssetMenu(fileName = "QuestItem", menuName = "Scriptable Objects/Items/QuestItem")]
    public class QuestItem : BaseItem
    {
        public string objectiveId;
    }
}