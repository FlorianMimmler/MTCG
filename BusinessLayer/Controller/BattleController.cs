using MTCG.BusinessLayer.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.BusinessLayer;
using MTCG.BusinessLayer.Model.BattleStrategy;

namespace MTCG
{
    internal class BattleController
    {
        private IBattleStrategy BattleStrategy { get; set; }

        public BattleController(User player1, User player2)
        {
            this.Player1 = player1;
            this.Player2 = player2;
        }

        public User Player1 { get; set; }
        public User Player2 { get; set; }

        private const int MaxBattleRounds = 100;

        public void StartBattle()
        {
            Console.WriteLine($"--------| Start Battle |--------");
            Console.WriteLine($"{Player1.GetName()} Vs {Player2.GetName()}");
            for (var battleRoundIndex = 0; battleRoundIndex < MaxBattleRounds; battleRoundIndex++)
            {
                Console.WriteLine($">> Round {battleRoundIndex} <<");
                ProcessBattleRound();

                //Check Decks for winner
                if (IsBattleOver())
                {
                    break;
                }
            }

            ProcessBattleResult();
        }

        private void ProcessBattleRound()
        {
            //pick two random Cards
            var card1 = Player1.GetRandomCardFromDeck();
            var card2 = Player2.GetRandomCardFromDeck();

            Console.WriteLine($"{card1.Name} Vs {card2.Name}");

            //Set Strategy
            SetBattleRoundStrategy(card1, card2);

            //Execute Strategy
            var result = this.BattleStrategy.Execute(card1, card2);

            //Process Winner
            ProcessBattleRoundResult(result, card1, card2);
        }

        private void SetBattleRoundStrategy(ICard card1, ICard card2)
        {
            switch (card1)
            {
                case MonsterCard _ when card2 is MonsterCard:
                    this.BattleStrategy = new MonsterBattleStrategy();
                    break;
                case SpellCard _ when card2 is SpellCard:
                    this.BattleStrategy = new SpellBattleStrategy();
                    break;
                default:
                    this.BattleStrategy = new MixedBattleStrategy();
                    break;
            }
        }

        private void ProcessBattleRoundResult(BattleResult result, ICard card1, ICard card2)
        {
            switch (result)
            {
                case BattleResult.Player1Wins:
                    Player1.GetDeck().AddCard(card2);
                    Player2.GetDeck().RemoveCard(card2);
                    Console.WriteLine($"{card1.Name} wins");
                    break;
                case BattleResult.Player2Wins:
                    Player2.GetDeck().AddCard(card1);
                    Player1.GetDeck().RemoveCard(card1);
                    Console.WriteLine($"{card2.Name} wins");
                    break;
                case BattleResult.Tie:
                    Console.WriteLine($"Tie");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(result), result, null);
            }
        }

        private bool IsBattleOver()
        {
            return Player1.GetDeck().IsEmpty() || Player2.GetDeck().IsEmpty();
        }

        private BattleResult GetBattleResult()
        {
            if (Player1.GetDeck().IsEmpty())
            {
                return BattleResult.Player2Wins;
            } else if (Player2.GetDeck().IsEmpty())
            {
                return BattleResult.Player1Wins;
            }
            else
            {
                return BattleResult.Tie;
            }
        }

        private void ProcessBattleResult()
        {
            var result = GetBattleResult();
            switch(result)
            {
                case BattleResult.Player1Wins:
                    Player1.Elo.Increase();
                    Player2.Elo.Decrease();
                    Console.WriteLine("Player 1 wins");
                    break;
                case BattleResult.Player2Wins:
                    Player2.Elo.Increase();
                    Player1.Elo.Decrease();
                    Console.WriteLine("Player 2 wins");
                    break;
                case BattleResult.Tie:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(result), result, null);
            }
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
