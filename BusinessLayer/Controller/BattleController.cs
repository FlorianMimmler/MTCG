using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG
{
    internal class BattleController
    {
        private static BattleController _instance;

        public static BattleController Instance => _instance ?? (_instance = new BattleController());

        private BattleController() {}

        public User Player1 { get; set; }
        public User Player2 { get; set; }
    }
}
