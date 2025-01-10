using MTCG.BusinessLayer.Model.Trading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.DataAccessLayer
{
    public interface ITradeRepository : IRepository<TradingDeal>
    {
        public Task<IEnumerable<TradingDeal>?> GetByUserId(int userId);
    }
}
