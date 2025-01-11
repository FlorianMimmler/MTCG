using MTCG.BusinessLayer.Model.Trading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.BusinessLayer.Model.RequestObjects
{
    public class TradeRequest
    {
        public int OfferedCardId { get; set; } = -1;
        public TradingDealRequirements Requirements { get; set; } = new TradingDealRequirements();
        
    }
}
