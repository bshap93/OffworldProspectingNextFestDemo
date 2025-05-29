using Domains.Items;
using Domains.Items.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Domains.UI
{
    public class PickupDisplayItem : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text itemName;
        [SerializeField] private TMP_Text quantity;

        public void Display(BaseItem item, int itemQuantity)
        {
            icon.sprite = item.ItemIcon;
            itemName.text = item.ItemName;
            quantity.text = itemQuantity.ToString();
        }

        public void AddQuantity(int itemQuantity)
        {
            quantity.text = (int.Parse(quantity.text) + itemQuantity).ToString();
        }
    }
}