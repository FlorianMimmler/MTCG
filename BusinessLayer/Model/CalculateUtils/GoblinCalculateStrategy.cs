using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.BusinessLayer;

namespace MTCG
{
    internal class GoblinCalculateStrategy : ICalculateStrategy
    {
        public double CalculateAgainst(Card oppponent)
        {
            
            
            return 0;
        }

        public double CalculateAgainstMonster(Card oppponent, MonsterType opponentMonster)
        {

            if (opponentMonster == MonsterType.Dragon)
            {
                return 0;
            }

            return 1;
        }
    }
}
