using MTCG.BusinessLayer.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.BusinessLayer.Model.Trading
{
    internal class TradingDealRequest(int tradeId, int userId, ICard offeredCard, int? id = null)
    {
        public int? Id { get; set; } = id;
        public int TradeId { get; set; } = tradeId;
        public ICard OfferedCard { get; set; } = offeredCard;
        public int RequestUserId { get; set; } = userId;
    }
}
