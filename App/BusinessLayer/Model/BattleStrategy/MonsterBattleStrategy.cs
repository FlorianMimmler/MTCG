using MTCG.BusinessLayer.Interface;
using MTCG.BusinessLayer.Model.Card;

namespace MTCG.BusinessLayer.Model.BattleStrategy
{
    internal class MonsterBattleStrategy : BattleStrategy
    {
        private readonly SpecialCaseResolver _specialCaseResolver = new();

        public override BattleResult Execute(ICard card1, ICard card2)
        {
            var monster1 = card1 as MonsterCard;
            var monster2 = card2 as MonsterCard;

            var specialResult = _specialCaseResolver.Resolve(monster1, monster2);

            return specialResult != BattleResult.Tie ? specialResult : CompareDamage(monster1.Damage, monster2.Damage);
        }
    }

}
