using MTCG.BusinessLayer.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.BusinessLayer.Model
{
    public abstract class ShopItem : IShopItem
    {
        public ShopItem() 
        {
            Id = -1;
            Name = "";
        }

        public ShopItem(int id, string name, string? description, int price, ShopItemType type = ShopItemType.Unkown)
        {
            Id = id;
            Name = name;
            Type = type;
            Description = description;
            Price = price;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public ShopItemType Type { get; set; } = ShopItemType.Unkown;
        public string? Description { get; set; }
        public int Price { get; set; }

        internal static ShopItemType StringTypeToEnum(string type)
        {
            return type switch
            {
                "mysterypack" => ShopItemType.MysteryPack,
                _ => ShopItemType.Unkown,
            };
        }

        internal static string EnumToString(ShopItemType type)
        {
            return type switch
            {
                ShopItemType.MysteryPack => "mysterypack",
                _ => "unknown",
            };
        }
    }

    public enum ShopItemType
    {
        MysteryPack,
        Unkown
    }
}
