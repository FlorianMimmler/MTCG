using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.BusinessLayer.Model.User
{
    internal class Stats
    {

        public int Wins { get; set; } = 0;
        public int Losses { get; set; } = 0;

        public Elo Elo = new Elo();

        public void AddWin()
        {
            Wins++;
            Elo.Increase();
        }

        public void AddLoss()
        {
            Losses++;
            Elo.Decrease();
        }
    }
}
