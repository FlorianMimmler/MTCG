﻿using MTCG.BusinessLayer.Interface;
using MTCG.BusinessLayer.Model.BattleStrategy;
using System.Text.Json;
using MTCG.BusinessLayer.Model.Card;
using MTCG.Auth;
using MTCG.BusinessLayer.Model.User;

namespace MTCG.BusinessLayer.Controller
{
    public class BattleController
    {
        private IBattleStrategy BattleStrategy { get; set; } = new MixedBattleStrategy();

        public BattleController(IUser player1, IUser player2)
        {
            Player1 = player1;
            Player2 = player2;
        }

        public BattleController(IUser player)
        {
            Player1 = player;
            Player2 = CreateAiPlayer();
        }

        private User CreateAiPlayer()
        {
            var aiPlayer = new User(new Credentials("Ai-Player", "ai"))
            {
                Deck =
                {
                    Cards = CardController.Instance.GetCards(4)
                }
            };
            Ai = true;
            return aiPlayer;
        }

        private bool Ai = false;

        public IUser Player1 { get; set; }
        public IUser Player2 { get; set; }

        private const int MaxBattleRounds = 100;
        private int RoundsPlayed = 1;

        public List<BattleRoundLog> BattleLog { get; set; } = [];
        public BattleLogHeader BattleLogHeader { get; set; } = new();

        public async Task<bool> StartBattle()
        {
            try
            {
                Console.WriteLine($"--------| Start Battle |--------");
                Console.WriteLine($"{Player1.GetName()} Vs {Player2.GetName()}");

                BattleLogHeader = new BattleLogHeader
                {
                    Player1 = Player1.GetName(),
                    Player2 = Player2.GetName()
                };

                for (; RoundsPlayed <= MaxBattleRounds; RoundsPlayed++)
                {
                    Console.WriteLine($">> Round {RoundsPlayed} <<");
                    var roundLog = ProcessBattleRound(RoundsPlayed);

                    BattleLog.Add(roundLog);

                    //Check Decks for winner
                    if (IsBattleOver())
                    {
                        break;
                    }
                }

                _ = await ProcessBattleResult();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
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
            var resultData = ProcessBattleRoundResult(result, card1, card2);

            return new BattleRoundLog
            {
                RoundNumber = roundNumber,
                CardPlayer1 = card1,
                CardPlayer2 = card2,
                WinnerCard = resultData.Item2,
                Winner = resultData.Item1
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

        private Tuple<string, string> ProcessBattleRoundResult(BattleResult result, ICard card1, ICard card2)
        {
            switch (result)
            {
                case BattleResult.Player1Wins:
                    Player1.GetDeck().AddCard(card2);
                    Player2.GetDeck().RemoveCard(card2);
                    Console.WriteLine($"{card1.Name} wins");
                    return new Tuple<string, string>(Player1.GetName(), card1.Name);
                case BattleResult.Player2Wins:
                    Player2.GetDeck().AddCard(card1);
                    Player1.GetDeck().RemoveCard(card1);
                    Console.WriteLine($"{card2.Name} wins");
                    return new Tuple<string, string>(Player2.GetName(), card2.Name);
                case BattleResult.Tie:
                    Console.WriteLine($"Tie");
                    return new Tuple<string, string>("", "");
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

            if (Player2.GetDeck().IsEmpty())
            {
                return BattleResult.Player1Wins;
            }
            
            return BattleResult.Tie;
            
        }

        private async Task<int> ProcessBattleResult()
        {
            var result = GetBattleResult();
            switch (result)
            {
                case BattleResult.Player1Wins:
                    Player1.Stats.AddWin();
                    Player2.Stats.AddLoss();
                    Player1.Stats.Elo.Increase(GetEloPoints(1));
                    Player2.Stats.Elo.Decrease(GetEloPoints(2));
                    Console.WriteLine("Player 1 wins");
                    BattleLogHeader.Winner = Player1.GetName();
                    break;
                case BattleResult.Player2Wins:
                    Player2.Stats.AddWin();
                    Player1.Stats.AddLoss();
                    Player1.Stats.Elo.Decrease(GetEloPoints(1));
                    Player2.Stats.Elo.Increase(GetEloPoints(2));
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
            
            _ = await Player1.SaveStats();
            if (Ai) return 1;

            _ = await Player2.SaveStats();
            _ = await Player2.CheckAndUnlockAchievements();
            _ = await Player1.CheckAndUnlockAchievements();

            return 1;
        }

        public string GetSerializedBattleLogForPlayer(int player)
        {

            var battleLogObject = new
            {
                player1 = BattleLogHeader.Player1,
                player2 = BattleLogHeader.Player2,
                winner = BattleLogHeader.Winner,
                elo = GetEloPoints(player),
                newAchievements = player == 1 ? Player1.NewAchievements : Player2.NewAchievements,
                roundsCount = RoundsPlayed,
                rounds = BattleLog
            };

            var battleLogString = JsonSerializer.Serialize(battleLogObject);
            return battleLogString;
        }

        private int GetEloPoints(int player)
        {
            var battleResult = GetBattleResult();
            var baseElo = (player == 1 && battleResult == BattleResult.Player1Wins) ||
                          (player == 2 && battleResult == BattleResult.Player2Wins) ? 20 :
                (player == 1 && battleResult == BattleResult.Player2Wins) ||
                (player == 2 && battleResult == BattleResult.Player1Wins) ? 12 :
                0;
            if (Ai)
            {
                baseElo /= 5;
            }
            return baseElo / ((RoundsPlayed < 10 ? 10 : RoundsPlayed) / 10);
        }
    }

    public class BattleLogHeader
    {
        public string Player1 { get; set; } = "";
        public string Player2 { get; set; } = "";
        public string Winner { get; set; } = "";
    }

    public class BattleRoundLog
    {
        public int RoundNumber { get; set; } = 0;
        public ICard? CardPlayer1 { get; set; }
        public ICard? CardPlayer2 { get; set; }
        public string WinnerCard { get; set; } = "";
        public string Winner { get; set; } = "";
    }
}
