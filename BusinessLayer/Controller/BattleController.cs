using MTCG.BusinessLayer.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG
{
    internal class BattleController
    {
        private static BattleController _instance;

        public static BattleController Instance => _instance ?? (_instance = new BattleController());

        private BattleController() {}

        public User Player1 { get; set; }
        public User Player2 { get; set; }

        public void StartBattle(User player1, User player2)
        {
            //pick two random Cards

            //Calculate Damage
            //
        }

        private int BattleCards(ICard cardPlayer1, ICard cardPlayer2)
        {
            if (cardPlayer1 is SpellCard && cardPlayer2 is SpellCard)
            {
                var effectivenessCard1 = ElementEffectiveness.GetEffectiveness(cardPlayer1.ElementType, cardPlayer2.ElementType);
                var effectivenessCard2 = ElementEffectiveness.GetEffectiveness(cardPlayer2.ElementType, cardPlayer1.ElementType);

                var card1Damage = cardPlayer1.Damage * effectivenessCard1;
                var card2Damage = cardPlayer2.Damage * effectivenessCard2;

                if (card1Damage > card2Damage)
                {
                    return 1;
                }
                else if (card1Damage < card2Damage)
                {
                    return 2;
                }
                else
                {
                    return 0;
                }
            }
            else if (cardPlayer1 is MonsterCard && cardPlayer2 is MonsterCard)
            {
                int specialCaseResult = CalculateSpecialCase((MonsterCard)cardPlayer1, (MonsterCard)cardPlayer2);
                if (specialCaseResult != 0)
                {
                    return specialCaseResult;
                }

                if (cardPlayer1.Damage > cardPlayer2.Damage)
                {
                    return 1;
                }
                else if (cardPlayer1.Damage < cardPlayer2.Damage)
                {
                    return 2;
                }
                else
                {
                    return 0;
                }

            } else
            {

                if (cardPlayer1 is MonsterCard monsterCardPlayer1)
                {
                    if (monsterCardPlayer1.MonsterType == MonsterType.Knight && cardPlayer2 is SpellCard && cardPlayer2.ElementType == ElementType.WATER)
                    {
                        return 2;
                    }

                    if (monsterCardPlayer1.MonsterType == MonsterType.Kraken && cardPlayer2 is SpellCard)
                    {
                        return 1;
                    }
                }

                if (cardPlayer2 is MonsterCard monsterCardPlayer2)
                {
                    if (monsterCardPlayer2.MonsterType == MonsterType.Knight && cardPlayer1 is SpellCard && cardPlayer1.ElementType == ElementType.WATER)
                    {
                        return 1;
                    }

                    if (monsterCardPlayer2.MonsterType == MonsterType.Kraken && cardPlayer1 is SpellCard)
                    {
                        return 2;
                    }
                }

                var effectivenessCard1 = ElementEffectiveness.GetEffectiveness(cardPlayer1.ElementType, cardPlayer2.ElementType);
                var effectivenessCard2 = ElementEffectiveness.GetEffectiveness(cardPlayer2.ElementType, cardPlayer1.ElementType);

                var card1Damage = cardPlayer1.Damage * effectivenessCard1;
                var card2Damage = cardPlayer2.Damage * effectivenessCard2;

                if (card1Damage > card2Damage)
                {
                    return 1;
                }
                else if (card1Damage < card2Damage)
                {
                    return 2;
                }
                else
                {
                    return 0;
                }
            }
        }

        private int CalculateSpecialCase(MonsterCard cardPlayer1, MonsterCard cardPlayer2)
        {
            // Verwende Dictionary oder Lookup für verbesserte Lesbarkeit und Erweiterbarkeit
            var specialCases = new Dictionary<(MonsterType, MonsterType), int>
            {
                { (MonsterType.Goblin, MonsterType.Dragon), 2 },
                { (MonsterType.Wizzard, MonsterType.Ork), 1 },
                { (MonsterType.Elve, MonsterType.Dragon), 2 }
            };

            // Überprüfe beide Richtungen
            if (specialCases.TryGetValue((cardPlayer1.MonsterType, cardPlayer2.MonsterType), out int result))
            {
                return result;
            }
            if (specialCases.TryGetValue((cardPlayer2.MonsterType, cardPlayer1.MonsterType), out result))
            {
                return result == 1 ? 2 : 1; // Umkehrung des Ergebnisses für die entgegengesetzte Richtung
            }

            return 0; // Kein spezieller Fall, also Unentschieden
        }
    }
}
