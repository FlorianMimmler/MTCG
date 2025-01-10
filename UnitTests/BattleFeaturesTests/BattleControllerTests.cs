using MTCG.BusinessLayer.Controller;
using MTCG.BusinessLayer.Interface;
using MTCG.BusinessLayer.Model.Achievements;
using MTCG.BusinessLayer.Model.Card;
using MTCG.BusinessLayer.Model.CardWrapper;
using NSubstitute;

namespace MTCG_uTests.BattleFeaturesTests
{
    internal class BattleControllerTests
    {
        
        private IUser _player1;
        private IUser _player2;
        private BattleController _controller;

        private readonly string _player1Name = "Player1";
        private readonly string _player2Name = "Player2";

        [SetUp]
        public void SetUp()
        {
            _player1 = Substitute.For<IUser>();
            _player1.GetName().Returns(_player1Name);
            _player1.NewAchievements.Returns(new List<Achievement>());
            _player2 = Substitute.For<IUser>();
            _player2.GetName().Returns(_player2Name);

            // Mock the deck for both players
            _player1.GetDeck().Returns(new Deck
            {
                Cards = new List<ICard>
            {
                new MonsterCard(10, ElementType.NORMAL, MonsterType.Dragon) { Name = "Dragon1" },
                new MonsterCard(15, ElementType.NORMAL, MonsterType.Knight) { Name = "Knight1" }
            }
            });

            _player2.GetDeck().Returns(new Deck
            {
                Cards = new List<ICard>
            {
                new MonsterCard(20, ElementType.NORMAL, MonsterType.Goblin) { Name = "Goblin1" },
                new MonsterCard(5, ElementType.NORMAL, MonsterType.Elve) { Name = "Elve1" }
            }
            });

            _controller = new BattleController(_player1, _player2);
            CardController.Instance = null;
        }

        [Test]
        public void Constructor_TwoPlayers_ShouldInitializePlayersCorrectly()
        {
            // Act
            var controller = new BattleController(_player1, _player2);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(controller.Player1, Is.EqualTo(_player1));
                Assert.That(controller.Player2, Is.EqualTo(_player2));
            });
        }

        [Test]
        public void Constructor_WithAiPlayer_ShouldCreateAiPlayer()
        {
            // Arrange
            var cardControllerMock = Substitute.For<ICardController>();
            CardController.Instance = cardControllerMock;

            cardControllerMock.GetCards(4).Returns(
                [
                    new MonsterCard(10, ElementType.NORMAL, MonsterType.Dragon),
                    new SpellCard(20, ElementType.FIRE),
                    new MonsterCard(10, ElementType.NORMAL, MonsterType.Knight),
                    new SpellCard(20, ElementType.WATER)
                ]
             );

            // Act
            var controller = new BattleController(_player1);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(controller.Player2.GetName(), Is.EqualTo("Ai-Player"));
                Assert.That(controller.Player2.GetDeck().Cards.Count, Is.EqualTo(4));
            });
        }

        [Test]
        public async Task StartBattle_ShouldLogRoundsAndReturnTrue()
        {
            // Arrange
            _player1.GetRandomCardFromDeck().Returns(new MonsterCard(10, ElementType.NORMAL, MonsterType.Dragon));
            _player2.GetRandomCardFromDeck().Returns(new MonsterCard(5, ElementType.NORMAL, MonsterType.Goblin));

            // Act
            var result = await _controller.StartBattle();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.True);
                Assert.That(_controller.BattleLog, Is.Not.Empty);
                Assert.That(_controller.BattleLog.Count, Is.GreaterThan(0));
                Assert.That(_controller.BattleLog[0].Winner, Is.EqualTo(_player1Name));
            });
        }

        [Test]
        public void GetSerializedBattleLogForPlayer_ShouldReturnValidJson()
        {
            // Arrange
            _controller.BattleLogHeader = new BattleLogHeader
            {
                Player1 = "Player1",
                Player2 = "Player2",
                Winner = "Player1"
            };

            _controller.BattleLog.Add(new BattleRoundLog
            {
                RoundNumber = 1,
                CardPlayer1 = new MonsterCard(10, ElementType.NORMAL, MonsterType.Dragon),
                CardPlayer2 = new MonsterCard(5, ElementType.NORMAL, MonsterType.Goblin),
                Winner = "Player1",
                WinnerCard = "Dragon1"
            });

            // Act
            var json = _controller.GetSerializedBattleLogForPlayer(1);

            // Assert
            Assert.That(json, Does.Contain("\"player1\":\"Player1\""));
            Assert.That(json, Does.Contain("\"winner\":\"Player1\""));
            Assert.That(json, Does.Contain("\"roundsCount\":1"));
        }
        
    }
}
