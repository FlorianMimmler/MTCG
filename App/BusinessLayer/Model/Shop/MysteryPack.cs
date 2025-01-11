using MTCG.BusinessLayer.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.BusinessLayer.Model.Shop
{
    internal class MysteryPack : ShopItem
    {

        public MysteryPack(int id, string name, string? description, int price) : base(id, name, description, price, ShopItemType.MysteryPack) { }

        private readonly int CardPercentage = 20;
        private readonly int CoinsPercentage = 50;
        private readonly int NothingPercentage = 30;
        private static readonly Random RandomGen = new Random();
        public MysteryResult GetMystery()
        {
            int roll = RandomGen.Next(1, 101); // Random number between 1 and 100

            if (roll <= CardPercentage)
            {
                return new MysteryResult(true, "card", card: CardController.Instance.GetCard());
            }
            else if (roll <= CardPercentage + CoinsPercentage)
            {
                return new MysteryResult(true, "coins", coins: RandomGen.Next(7, 24));
            }
            else
            {
                return new MysteryResult(false, "nothing");
            }
        }

    }
}
