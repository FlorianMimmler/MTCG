using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.BusinessLayer.Model.User
{
    public class Stats
    {
        public int Id { get; set; }
        public int Wins { get; set; } = 0;
        public int Losses { get; set; } = 0;

        public Elo Elo = new Elo();

        public void AddWin()
        {
            Wins++;
        }

        public void AddLoss()
        {
            Losses++;
        }
    }
}
