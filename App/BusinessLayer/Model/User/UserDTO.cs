using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.BusinessLayer.Model.User
{
    internal class UserDTO
    {

        public string Username { get; set; }
        public int Coins { get; set; }
        public int EloPoints { get; set; }
        public string EloName { get; set; }
        public int CardCount { get; set; }

    }

    internal class UserStatsDTO
    {
        public string Username { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int EloPoints { get; set; }
        public string EloName { get; set; }
    }
}
