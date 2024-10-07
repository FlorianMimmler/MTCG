using MTCG.BusinessLayer.Interface;

namespace MTCG.BusinessLayer
{
    internal interface IBattleStrategy
    {
        BattleResult Execute(ICard card1, ICard card2);
    }
}
