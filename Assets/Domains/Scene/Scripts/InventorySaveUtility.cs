using System.Collections.Generic;

namespace Domains.Scene.Scripts
{
    public static class InventorySaveUtility
    {
        private const string InventoryKey = "InventoryContent";
        private const string WeightLimitKey = "InventoryMaxWeight";

        public static void Reset()
        {
            var path = "GameSave.es3";

            if (ES3.FileExists(path))
            {
                if (ES3.KeyExists(InventoryKey, path)) ES3.DeleteKey(InventoryKey, path);
                if (ES3.KeyExists(WeightLimitKey, path)) ES3.DeleteKey(WeightLimitKey, path);
            }
        }

        public static void Save(List<PlayerInventoryManager.InventoryEntryData> entries, float weightLimit)
        {
            var path = "GameSave.es3";
            ES3.Save(InventoryKey, entries, path);
            ES3.Save(WeightLimitKey, weightLimit, path);
        }
    }
}