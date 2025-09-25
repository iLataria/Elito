
using Bunker.Core.Services.Base;
using Bunker.Core.Services.Shop.Playfab.Base;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Core.Bunker.Services.Shop.Base
{
    public abstract class BaseShopService : CoreService
    {

        protected List<ShopItem> _shopItems = new();
        protected List<ShopBundleItem> _shopBundleItems = new();
        protected List<ShopContainerItem> _shopContainerItems = new();

        protected LootboxesData lootboxesData = new();
        protected DropTablesData dropTablesData = new();
        protected PlayFabRewards ingameRewards = new();
        protected GachaItemsList gachaItems = new();

        protected virtual UniTask CacheCatalogueItemsAsync()
        {
            return UniTask.CompletedTask;
        }
    }
}

