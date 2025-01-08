using MTCG.BusinessLayer.Interface;

namespace MTCG.BusinessLayer.Model.BattleStrategy
{
    internal interface IBattleStrategy
    {
        BattleResult Execute(ICard card1, ICard card2);
    }
}
