using MTCG.BusinessLayer.Interface;

namespace MTCG.BusinessLayer.Model.BattleStrategy
{
    internal abstract class BattleStrategy : IBattleStrategy
    {
        public abstract BattleResult Execute(ICard card1, ICard card2);

        public BattleResult CompareDamage(double damage1, double damage2)
        {
            if (damage1 > damage2) return BattleResult.Player1Wins;
            if (damage2 > damage1) return BattleResult.Player2Wins;
            return BattleResult.Tie;
        }

    }
}
