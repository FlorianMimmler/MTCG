using MTCG.BusinessLayer.Interface;
using MTCG.BusinessLayer.Model;
using MTCG.BusinessLayer.Model.Shop;
using MTCG.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.BusinessLayer.Controller
{
    internal class ShopController
    {

        private static ShopController? _instance;
        private List<IShopItem> _shopItems = [];

        public static ShopController Instance
        {
            get => _instance ??= new ShopController();
            set => _instance = value;
        }

        private ShopController()
        {
            LoadItemsSetup();
        }
        private async void LoadItemsSetup()
        {
            _shopItems = (await ShopRepository.Instance.GetAll())?.ToList() ?? [];
        }
        private async Task<List<IShopItem>> LoadItems()
        {
            return (await ShopRepository.Instance.GetAll())?.ToList() ?? [];
        }

        public async void UpdateShop()
        {
            _shopItems = await LoadItems();
        }

        public async Task<List<IShopItem>?> GetShopItems()
        {
            if(_shopItems.Count == 0)
            {
                _shopItems = await LoadItems();
            }
            Console.WriteLine(_shopItems[0]);
            return _shopItems;
        }

        /*public string BuyItem(int itemId)
        {
            var item = shopItems.FirstOrDefault(i => i is ShopItem shopItem && shopItem.Id == itemId);

            if (item == null)
            {
                return (-1, { "message" : "Item not found in the shop."});
            }

            if (item is MysteryPack mysteryPack)
            {
                var mysteryResult = mysteryPack.GetMystery();
                if (mysteryResult.IsSuccess)
                {
                    //user.ApplyMysteryResult(mysteryResult)

                    return mysteryResult.RewardType == "card" ? (1, { "message" : "you got one card", "card" : mysteryResult.Card }) : (1, { "message" : "you got " + mysteryResult.Coins + " Coins"});
                }
                return (0, { "message" : "you got nothing" });
                
            }

            return (-1, { "message" : "This item is not purchasable."});
        }*/
        public string BuyItem(int itemId)
        {
            var item = _shopItems.FirstOrDefault(i => i is ShopItem shopItem && shopItem.Id == itemId);

            if (item == null)
            {
                return "Item not found in the shop.";
            }

            if (item is MysteryPack mysteryPack)
            {
                var mysteryResult = mysteryPack.GetMystery();
                if (mysteryResult.IsSuccess)
                {
                    //user.ApplyMysteryResult(mysteryResult)

                    
                }
                return mysteryResult.ToString();

            }

            return "This item is not purchasable.";
        }
    }
}
