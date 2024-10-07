using MTCG.BusinessLayer.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.BusinessLayer.Model.BattleStrategy;

namespace MTCG.BusinessLayer
{
    internal interface IBattleStrategy
    {
        BattleResult Execute(ICard card1, ICard card2);
    }
}
