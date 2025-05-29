#if !DISABLESTEAMWORKS && STEAMWORKSNET
using System.Collections.Generic;
using System.Linq;
using Heathen.SteamworksIntegration;
using Heathen.SteamworksIntegration.API;
using UnityEngine;

namespace HeathenEngieering.SteamworksIntegration
{
    public class InventoryManager : MonoBehaviour
    {
        public InventoryChangedEvent evtChanged;
        public SteamMicroTransactionAuthorizationResponce evtTransactionResponce;
        public Currency.Code CurrencyCode => Inventory.Client.LocalCurrencyCode;
        public string CurrencySymbol => Inventory.Client.LocalCurrencySymbol;

        public List<ItemDefinitionObject> Items
        {
            get
            {
                if (SteamSettings.current != null)
                {
                    return SteamSettings.Client.inventory.items;
                }

                Debug.LogWarning("You can only fetch the list of items if your using a SteamSettings object");
                return null;
            }
        }

        private void OnEnable()
        {
            if (SteamSettings.current != null)
                SteamSettings.Client.inventory.EventChanged.AddListener(evtChanged.Invoke);
            Inventory.Client.EventSteamMicroTransactionAuthorizationResponse.AddListener(evtTransactionResponce.Invoke);
        }

        private void OnDisable()
        {
            if (SteamSettings.current != null)
                SteamSettings.Client.inventory.EventChanged.RemoveListener(evtChanged.Invoke);
            Inventory.Client.EventSteamMicroTransactionAuthorizationResponse.RemoveListener(evtTransactionResponce
                .Invoke);
        }

        /// <summary>
        ///     Returns the sub set of items that have a price and are not hidden.
        ///     These should be the same items visible in Steam's store
        /// </summary>
        /// <returns></returns>
        public ItemDefinitionObject[] GetStoreItems()
        {
            return Items.Where(i => !i.Hidden && !i.StoreHidden && i.item_price.Valid).ToArray();
        }
    }
}
#endif