using MTCG.BusinessLayer.Interface;

namespace MTCG.BusinessLayer.Model.BattleStrategy
{
    internal class SpellBattleStrategy : BattleStrategy
    {

        public override BattleResult Execute(ICard card1, ICard card2)
        {
            var effectiveness1 = ElementEffectiveness.GetEffectiveness(card1.ElementType, card2.ElementType);
            var effectiveness2 = ElementEffectiveness.GetEffectiveness(card2.ElementType, card1.ElementType);

            var damage1 = card1.Damage * effectiveness1;
            var damage2 = card2.Damage * effectiveness2;

            return CompareDamage(damage1, damage2);
        }

    }
}
