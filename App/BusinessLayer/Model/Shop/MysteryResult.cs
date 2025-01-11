using MTCG.BusinessLayer.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.BusinessLayer.Model.Shop
{
    public class MysteryResult
    {
        public bool IsSuccess { get; set; }
        public string RewardType { get; set; }
        public int Coins { get; set; }
        public ICard? Card { get; set; }

        public MysteryResult(bool isSuccess, string rewardType, int coins = 0, ICard? card = null)
        {
            IsSuccess = isSuccess;
            RewardType = rewardType;
            Coins = coins;
            Card = card;
        }
    }
}
