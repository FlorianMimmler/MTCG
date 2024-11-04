using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.BusinessLayer.Model.Trading
{
    internal class TradingDeal(MTCG.Card offeredCard, TradingDealRequirements requirements, int userId)
    {

        public MTCG.Card OfferedCard { get; set; } = offeredCard;
        public TradingDealRequirements Requirements { get; set; } = requirements;
        public int OfferingUserId { get; set; } = userId;


    }
}
