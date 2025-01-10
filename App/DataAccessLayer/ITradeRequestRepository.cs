using MTCG.BusinessLayer.Model.Trading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.DataAccessLayer
{
    public interface ITradeRequestRepository : IRepository<TradingDealRequest>
    {
        public Task<IEnumerable<TradingDealRequest>?> GetByTradeId(int id);
    }
}
