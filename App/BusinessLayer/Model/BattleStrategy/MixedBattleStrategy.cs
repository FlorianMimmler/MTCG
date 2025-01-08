using MTCG.BusinessLayer.Interface;

namespace MTCG.BusinessLayer.Model.BattleStrategy
{
    internal class MixedBattleStrategy : BattleStrategy
    {
        public override BattleResult Execute(ICard card1, ICard card2)
        {
            if (card1 is MonsterCard monster && card2 is SpellCard spell)
            {
                if (monster.MonsterType == MonsterType.Knight && spell.ElementType == ElementType.WATER)
                    return BattleResult.Player2Wins;
                if (monster.MonsterType == MonsterType.Kraken)
                    return BattleResult.Player1Wins;
            }

            if (card2 is MonsterCard monster2 && card1 is SpellCard spell2)
            {
                if (monster2.MonsterType == MonsterType.Knight && spell2.ElementType == ElementType.WATER)
                    return BattleResult.Player1Wins;
                if (monster2.MonsterType == MonsterType.Kraken)
                    return BattleResult.Player2Wins;
            }

            var effectiveness1 = ElementEffectiveness.GetEffectiveness(card1.ElementType, card2.ElementType);
            var effectiveness2 = ElementEffectiveness.GetEffectiveness(card2.ElementType, card1.ElementType);

            var damage1 = card1.Damage * effectiveness1;
            var damage2 = card2.Damage * effectiveness2;

            return CompareDamage(damage1, damage2);
        }
    }
}
