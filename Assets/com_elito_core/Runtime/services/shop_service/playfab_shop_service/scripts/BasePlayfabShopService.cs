using Core.Bunker.Services.Shop.Base;
using Bunker.Core.Services.SPlayFab.Extensions;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Bunker.Core.Services.Shop.Playfab.Base
{
    public abstract class BasePlayfabShopService : BaseShopService
    {
        const string NUTAKU_COIN_ID = "NG";
        const string STORE_ID = "main_store";
        const string CATALOG_VERSION = "sexy_b_sep24";

    

        public GachaItemsList GachaItems => gachaItems;

        protected async override UniTask CacheCatalogueItemsAsync()
        {
            var getCatalogItemsRequest = new GetCatalogItemsRequest
            {
                CatalogVersion = CATALOG_VERSION
            };

            var getStoreItemsRequest = new GetStoreItemsRequest
            {
                CatalogVersion = CATALOG_VERSION,
                StoreId = STORE_ID
            };

            var getCatalogItemsResult = await PlayFabClientAPIAsync.GetCatalogItemsAsync(getCatalogItemsRequest);

            var getStoreItemsResult = await PlayFabClientAPIAsync.GetStoreItemsAsync(getStoreItemsRequest);

            var getTitleDataResult = await PlayFabClientAPIAsync.GetTitleDataAsync(new GetTitleDataRequest());

            lootboxesData = JsonUtility.FromJson<LootboxesData>(getTitleDataResult.Data["lootboxes"]);
            dropTablesData = JsonUtility.FromJson<DropTablesData>(getTitleDataResult.Data["DropTables"]);

            foreach (var storeItem in getStoreItemsResult.Store)
            {
                var catalogItem = getCatalogItemsResult.Catalog.FirstOrDefault(o => o.ItemId == storeItem.ItemId);

                if (catalogItem == null)
                    continue;

                var customData = GetCustomDataDetails(storeItem.CustomData);

                if (Enum.TryParse(catalogItem.ItemClass, out InventoryItemClass itemClass))
                {
                    var shopItem = new ShopItem(storeItem.ItemId, itemClass, catalogItem.Description, catalogItem.ItemImageUrl, storeItem.VirtualCurrencyPrices, catalogItem.VirtualCurrencyPrices, customData);

                    _shopItems.Add(shopItem);

                    continue;
                }

                if (Enum.TryParse(catalogItem.ItemClass, out BundleItemClass itemBundleClass))
                {
                    var shopBundleItem = new ShopBundleItem(storeItem.ItemId, itemBundleClass, catalogItem.Description, catalogItem.ItemImageUrl, catalogItem.Bundle.BundledVirtualCurrencies ?? new Dictionary<string, uint>(), catalogItem.Bundle.BundledItems, storeItem.VirtualCurrencyPrices, catalogItem.VirtualCurrencyPrices, customData);

                    _shopBundleItems.Add(shopBundleItem);

                    continue;
                }

                if (Enum.TryParse(catalogItem.ItemClass, out ContainerItemClass itemContainerClass))
                {
                    var lootboxItem = lootboxesData.Lootboxes.FirstOrDefault(o => o.Lootbox_Id == storeItem.ItemId);

                    var shopContainerItem = new ShopContainerItem(storeItem.ItemId, itemContainerClass, catalogItem.Description, catalogItem.ItemImageUrl, storeItem.VirtualCurrencyPrices, catalogItem.VirtualCurrencyPrices, lootboxItem, customData);

                    _shopContainerItems.Add(shopContainerItem);
                }

            }
        }

        static CustomData GetCustomDataDetails(object data)
        {
            var customData = new CustomData();

            if (data == null)
                return customData;

            var dataString = data.ToString();

            if (string.IsNullOrEmpty(dataString))
            {
                return customData;
            }

            customData = JsonConvert.DeserializeObject<CustomData>(dataString);

            Debug.Log($"Label Id: {customData.labelId}");

            return customData;
        }
    }

    [Serializable]
    public class LootboxesData
    {
        public List<LootboxItem> Lootboxes;
    }

    [Serializable]
    public class LootboxItem
    {
        public string Lootbox_Id;
        public int Guarantee_Drop_Count;
        public string Guarantee_Drop_Id;
        public int Drop_Count;
        public List<LootboxItemDrop> Drop;
    }

    [Serializable]
    public class LootboxItemDrop
    {
        public string Drop_Id;
        public int Weight;
    }

    [Serializable]
    public class DropTablesData
    {
        public List<DropTableItem> DropTables;
    }

    [Serializable]
    public class DropTableItem
    {
        public string TableId;
        public string Rarity;
        public List<DropTableNode> Nodes;
    }

    [Serializable]
    public class DropTableNode
    {
        public string ResultItemType;
        public string ResultItem;
        public int Weight;
    }

    [Serializable]
    public enum InventoryItemClass
    {
        ActionItems,
        Boosters,
        ClickerEnters,
        ClickerEventEnters,
        Cosmetics,
        GachaItems,
        GirlDresses,
        MiniGameEnters,
        MosaicPoints,
        WheelEventEnters,
    }

    [Serializable]
    public enum BundleItemClass
    {
        GoldPacks,
        Offers,
        UpsellOffers,
    }

    [Serializable]
    public enum ContainerItemClass
    {
        Lootboxes
    }

    [Serializable]
    public class CustomData
    {
        public string labelId = string.Empty;
    }

    [Serializable]
    public class PlayFabRewards
    {
        public List<PlayFabReward> ingameRewards = new();
    }

    [Serializable]
    public class PlayFabReward
    {
        public string rewardId = string.Empty;

        public bool isMulti = false;

        public Dictionary<string, long> rewardItems = new();
    }

    public class GachaItemsList
    {
        public List<GachaItem> gachaItems { get; } = new();
    }

    public class GachaItem
    {
        public string ItemId { get; set; }

        public bool IsUnique { get; set; }

        public Dictionary<string, int> RewardItems { get; } = new();
    }

    [Serializable]
    public class ShopItem
    {
        [SerializeField]
        string _itemId;

        [SerializeField]
        InventoryItemClass _itemClass;

        [SerializeField]
        string _nameKey;

        [SerializeField]
        string _iconUrl;

        Dictionary<string, uint> _currentPrice;

        Dictionary<string, uint> _basePrice;

        CustomData _customData;

        public string ItemId => _itemId;

        public InventoryItemClass ItemClass => _itemClass;

        public string NameKey => _nameKey;

        public string IconUrl => _iconUrl;

        public Dictionary<string, uint> CurrentPrice => _currentPrice;

        public Dictionary<string, uint> BasePrice => _basePrice;

        public CustomData CustomData => _customData;

        public ShopItem(string itemId, InventoryItemClass itemClass, string nameKey, string iconUrl, Dictionary<string, uint> currentPrice, Dictionary<string, uint> basePrice, CustomData customData)
        {
            _itemId = itemId;
            _itemClass = itemClass;
            _nameKey = nameKey;
            _iconUrl = iconUrl;
            _currentPrice = currentPrice;
            _basePrice = basePrice;
            _customData = customData;
        }
    }

    [Serializable]
    public class ShopBundleItem
    {
        [SerializeField]
        string _id;

        [SerializeField]
        BundleItemClass _itemClass;

        [SerializeField]
        string _nameKey;

        [SerializeField]
        string _iconUrl;

        Dictionary<string, uint> _content;

        Dictionary<string, uint> _currentPrice;

        Dictionary<string, uint> _basePrice;

        CustomData _customData;

        public string ID => _id;

        public BundleItemClass ItemClass => _itemClass;

        public string NameKey => _nameKey;

        public string IconUrl => _iconUrl;

        public Dictionary<string, uint> Content => _content;

        public Dictionary<string, uint> CurrentPrice => _currentPrice;

        public Dictionary<string, uint> BasePrice => _basePrice;

        public CustomData CustomData => _customData;

        public ShopBundleItem(string id, BundleItemClass itemClass, string nameKey, string iconUrl, Dictionary<string, uint> currenciesContent, List<string> content, Dictionary<string, uint> currentPrice, Dictionary<string, uint> basePrice, CustomData customData)
        {
            _id = id;
            _itemClass = itemClass;
            _nameKey = nameKey;
            _iconUrl = iconUrl;
            _currentPrice = currentPrice;
            _basePrice = basePrice;
            _customData = customData;

            var itemContent = new Dictionary<string, uint>();

            //if (currenciesContent.Count > 0)
            //    itemContent.AddRange(currenciesContent);

            //if (content.Count > 0)
            //    itemContent.AddRange(content.GroupBy(o => o).ToDictionary(oo => oo.Key, oo => Convert.ToUInt32(oo.Count())));

            _content = itemContent;
        }
    }

    [Serializable]
    public class ShopContainerItem
    {
        [SerializeField]
        string _id;

        [SerializeField]
        ContainerItemClass _itemClass;

        [SerializeField]
        string _nameKey;

        [SerializeField]
        string _iconUrl;

        LootboxItem _content;

        Dictionary<string, uint> _currentPrice;

        Dictionary<string, uint> _basePrice;

        CustomData _customData;

        public string ID => _id;

        public ContainerItemClass ItemClass => _itemClass;

        public string NameKey => _nameKey;

        public string IconUrl => _iconUrl;

        public Dictionary<string, uint> CurrentPrice => _currentPrice;

        public Dictionary<string, uint> BasePrice => _basePrice;

        public CustomData CustomData => _customData;

        public LootboxItem Content => _content;

        public ShopContainerItem(string id, ContainerItemClass itemClass, string nameKey, string iconUrl, Dictionary<string, uint> currentPrice, Dictionary<string, uint> basePrice, LootboxItem content, CustomData customData)
        {
            _id = id;
            _itemClass = itemClass;
            _nameKey = nameKey;
            _iconUrl = iconUrl;
            _currentPrice = currentPrice;
            _basePrice = basePrice;
            _customData = customData;
            _content = content;
        }
    }
}
