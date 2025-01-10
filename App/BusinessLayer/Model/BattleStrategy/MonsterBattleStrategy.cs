using MTCG.BusinessLayer.Interface;
using MTCG.BusinessLayer.Model.Card;

namespace MTCG.BusinessLayer.Model.BattleStrategy
{
    public class MonsterBattleStrategy : BattleStrategy
    {
        public ISpecialCaseResolver SpecialCaseResolver { get;  set; } = new SpecialCaseResolver();

        public override BattleResult Execute(ICard card1, ICard card2)
        {
            var monster1 = card1 as MonsterCard;
            var monster2 = card2 as MonsterCard;

            if (monster1 == null || monster2 == null)
            {
                return BattleResult.Tie;
            }

            var specialResult = SpecialCaseResolver.Resolve(monster1, monster2);

            return specialResult != BattleResult.Tie ? specialResult : CompareDamage(monster1.Damage, monster2.Damage);
        }
    }

}
