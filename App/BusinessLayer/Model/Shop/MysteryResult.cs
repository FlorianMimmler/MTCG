using MTCG.BusinessLayer.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.BusinessLayer.Model.Shop
{
    internal class MysteryResult
    {
        public bool IsSuccess { get; set; } // Indicates if the user got something
        public string RewardType { get; set; } // e.g., "Card", "Coins", or "Nothing"
        public int Coins { get; set; } // Amount of coins (if applicable)
        public ICard? Card { get; set; } // The card's name or ID (if applicable)

        public MysteryResult(bool isSuccess, string rewardType, int coins = 0, ICard? card = null)
        {
            IsSuccess = isSuccess;
            RewardType = rewardType;
            Coins = coins;
            Card = card;
        }
    }
}
