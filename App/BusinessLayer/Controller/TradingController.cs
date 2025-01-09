using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.BusinessLayer.Model.Trading;

namespace MTCG.BusinessLayer.Controller
{
    internal class TradingController
    {
        private static TradingController? _instance;
        public static TradingController Instance => _instance ??= new TradingController();

        private TradingController()
        {
        }
    }
}
