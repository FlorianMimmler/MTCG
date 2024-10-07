using System.Collections.Generic;

namespace MTCG.BusinessLayer
{
    internal class SpecialCaseResolver
    {
        private readonly Dictionary<(MonsterType, MonsterType), BattleResult> _specialCases =
            new Dictionary<(MonsterType, MonsterType), BattleResult>
            {
                { (MonsterType.Goblin, MonsterType.Dragon), BattleResult.Player2Wins },
                { (MonsterType.Wizzard, MonsterType.Ork), BattleResult.Player1Wins },
                { (MonsterType.Elve, MonsterType.Dragon), BattleResult.Player2Wins }
            };

        public BattleResult Resolve(MonsterCard card1, MonsterCard card2)
        {
            if (card1 == null || card2 == null)
                return BattleResult.Tie;

            if (_specialCases.TryGetValue((card1.MonsterType, card2.MonsterType), out var result))
                return result;

            if (_specialCases.TryGetValue((card2.MonsterType, card1.MonsterType), out result))
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
