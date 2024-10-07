using MTCG.BusinessLayer.Interface;
using System;
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
    }
}
