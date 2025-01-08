using MTCG.BusinessLayer.Model.Card;
using System.Collections.Generic;

namespace MTCG.BusinessLayer.Model.BattleStrategy
{
    public class SpecialCaseResolver
    {
        private readonly Dictionary<((MonsterType, ElementType?), MonsterType), BattleResult> _specialCases =
            new Dictionary<((MonsterType, ElementType?), MonsterType), BattleResult>
            {
                { ((MonsterType.Goblin, null), MonsterType.Dragon), BattleResult.Player2Wins },
                { ((MonsterType.Wizzard, null), MonsterType.Ork), BattleResult.Player1Wins },
                { ((MonsterType.Elve, ElementType.FIRE), MonsterType.Dragon), BattleResult.Player1Wins }
            };

        public BattleResult Resolve(MonsterCard? card1, MonsterCard? card2)
        {
            if (card1 == null || card2 == null)
                return BattleResult.Tie;

            if (_specialCases.TryGetValue(((card1.MonsterType, card1.MonsterType == MonsterType.Elve ? card1.ElementType : null), card2.MonsterType), out var result))
                return result;

            if (_specialCases.TryGetValue(((card2.MonsterType, card2.MonsterType == MonsterType.Elve ? card2.ElementType : null), card1.MonsterType), out result))
                return result switch
                {
                    BattleResult.Player1Wins => BattleResult.Player2Wins,
                    BattleResult.Player2Wins => BattleResult.Player1Wins,
                    _ => BattleResult.Tie
                };

            return BattleResult.Tie;
        }
    }
}
