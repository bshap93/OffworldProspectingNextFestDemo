using System;
using UnityEngine;

namespace Domains.Items.Scripts
{
    [Serializable]
    public enum ItemType
    {
        Ore,
        Bundle,
        Stone,
        Crystal,
        Keycard,
    }

    [CreateAssetMenu(fileName = "BaseItem", menuName = "Scriptable Objects/Items/BaseItem")]
    public class BaseItem : ScriptableObject
    {
        public string ItemID;

        public string ItemName;

        public string ItemDescription;

        public Sprite ItemIcon;

        public float ItemWeight;

        public int ItemValue;

        public ItemType ItemType;

        public virtual bool Pick()
        {
            return true;
        }


        public static bool IsNull(BaseItem baseItem)
        {
            if (baseItem == null) return true;
            if (baseItem.ItemID == null) return true;
            if (baseItem.ItemID == "") return true;
            return false;
        }
    }
}