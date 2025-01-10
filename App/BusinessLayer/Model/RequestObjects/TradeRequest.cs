using MTCG.BusinessLayer.Model.Trading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.BusinessLayer.Model.RequestObjects
{
    internal class TradeRequest
    {
        public int OfferedCardId { get; set; }
        public TradingDealRequirements Requirements { get; set; } = new TradingDealRequirements();
        
    }
}
