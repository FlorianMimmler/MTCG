using MTCG.BusinessLayer;
using MTCG.BusinessLayer.Interface;
using MTCG.BusinessLayer.Model.BattleStrategy;
using System;
using System.Text.Json;
using System.Xml;

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

        public List<BattleRoundLog> BattleLog { get; set; } = [];
        public BattleLogHeader BattleLogHeader { get; set; }

        public void StartBattle()
        {
            Console.WriteLine($"--------| Start Battle |--------");
            Console.WriteLine($"{Player1.GetName()} Vs {Player2.GetName()}");

            BattleLogHeader = new BattleLogHeader
            {
                Player1 = Player1.GetName(),
                Player2 = Player2.GetName()
            };

            for (var battleRoundIndex = 1; battleRoundIndex <= MaxBattleRounds; battleRoundIndex++)
            {
                Console.WriteLine($">> Round {battleRoundIndex} <<");
                var roundLog = ProcessBattleRound(battleRoundIndex);

                BattleLog.Add(roundLog);

                //Check Decks for winner
                if (IsBattleOver())
                {
                    break;
                }
            }

            ProcessBattleResult();
        }

        private BattleRoundLog ProcessBattleRound(int roundNumber)
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
            var winnerCard = ProcessBattleRoundResult(result, card1, card2);

            return new BattleRoundLog
            {
                RoundNumber = roundNumber,
                CardPlayer1 = card1.Name,
                CardPlayer2 = card2.Name,
                WinnerCard = winnerCard
            };
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

        private string ProcessBattleRoundResult(BattleResult result, ICard card1, ICard card2)
        {
            switch (result)
            {
                case BattleResult.Player1Wins:
                    Player1.GetDeck().AddCard(card2);
                    Player2.GetDeck().RemoveCard(card2);
                    Console.WriteLine($"{card1.Name} wins");
                    return card1.Name;
                case BattleResult.Player2Wins:
                    Player2.GetDeck().AddCard(card1);
                    Player1.GetDeck().RemoveCard(card1);
                    Console.WriteLine($"{card2.Name} wins");
                    return card2.Name;
                case BattleResult.Tie:
                    Console.WriteLine($"Tie");
                    return "Tie";
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
            }
            else if (Player2.GetDeck().IsEmpty())
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
            switch (result)
            {
                case BattleResult.Player1Wins:
                    Player1.Stats.AddWin();
                    Player2.Stats.AddLoss();
                    Console.WriteLine("Player 1 wins");
                    BattleLogHeader.Winner = Player1.GetName();
                    break;
                case BattleResult.Player2Wins:
                    Player2.Stats.AddWin();
                    Player1.Stats.AddLoss();
                    Console.WriteLine("Player 2 wins");
                    BattleLogHeader.Winner = Player2.GetName();
                    break;
                case BattleResult.Tie:
                    Console.WriteLine("Tie");
                    BattleLogHeader.Winner = "Tie";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(result), result, null);
            }
        }

        public string GetSerializedBattleLog()
        {
            var battleLogObject = new
            {
                player1 = BattleLogHeader.Player1,
                player2 = BattleLogHeader.Player2,
                winner = BattleLogHeader.Winner,
                rounds = BattleLog
            };

            var battleLogString = JsonSerializer.Serialize(battleLogObject);
            return battleLogString;
        }
    }

    internal class BattleLogHeader
    {
        public string Player1 { get; set; }
        public string Player2 { get; set; }
        public string Winner { get; set; }
    }

    internal class BattleRoundLog
    {
        public int RoundNumber { get; set; }
        public string CardPlayer1 { get; set; }
        public string CardPlayer2 { get; set; }
        public string WinnerCard { get; set; }
    }
}
