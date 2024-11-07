using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.BusinessLayer.Interface;
using MTCG.DataAccessLayer;

namespace MTCG.BusinessLayer.Model.Trading
{
    internal class TradingDeal(ICard offeredCard, TradingDealRequirements? requirements, int userId, int? id = null)
    {
        public int? Id { get; set; } = id;
        public ICard OfferedCard { get; set; } = offeredCard;
        public TradingDealRequirements? Requirements { get; set; } = requirements;
        public int OfferingUserId { get; set; } = userId;
    }
}
