using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.BusinessLayer.Model.RequestObjects
{
    public class TradeRequestRequest
    {
        public int TradeId { get; set; }
        public int OfferedCardId { get; set; }

    }
}
