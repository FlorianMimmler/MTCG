using MTCG.BusinessLayer.Interface;
using MTCG.BusinessLayer.Model.BattleStrategy;
using MTCG.BusinessLayer.Model.Card;
using NSubstitute;

namespace MTCG_uTests.BattleStrategy
{
    internal class MonsterStrategyTests
    {

        private ISpecialCaseResolver _specialCaseResolverMock;
        private MonsterBattleStrategy _strategy;

        [SetUp]
        public void SetUp()
        {
            _specialCaseResolverMock = Substitute.For<ISpecialCaseResolver>();
            _strategy = new MonsterBattleStrategy { SpecialCaseResolver = _specialCaseResolverMock };
        }

        [Test]
        public void Execute_SpecialCaseReturnsPlayer1Wins_ShouldReturnPlayer1Wins()
        {
            // Arrange
            var greaterDamage = 50;
            var lowerDamage = 40;
            var monster1 = new MonsterCard(greaterDamage, ElementType.NORMAL, MonsterType.Dragon);
            var monster2 = new MonsterCard(lowerDamage, ElementType.NORMAL, MonsterType.Ork);
            _specialCaseResolverMock.Resolve(monster1, monster2).Returns(BattleResult.Player1Wins);

            // Act
            var result = _strategy.Execute(monster1, monster2);

            // Assert
            Assert.That(result, Is.EqualTo(BattleResult.Player1Wins));
            _specialCaseResolverMock.Received(1).Resolve(monster1, monster2);
        }

        [Test]
            public void Execute_SpecialCaseReturnsTie_ShouldCallCompareDamage()
            {
                // Arrange
                var greaterDamage = 50;
                var lowerDamage = 40;
                var monster1 = new MonsterCard(greaterDamage, ElementType.NORMAL, MonsterType.Dragon);
                var monster2 = new MonsterCard(lowerDamage, ElementType.NORMAL, MonsterType.Dragon);
                _specialCaseResolverMock.Resolve(monster1, monster2).Returns(BattleResult.Tie);

                // Act
                var result = _strategy.Execute(monster1, monster2);

                // Assert
                Assert.That(result, Is.EqualTo(BattleResult.Player1Wins)); // This ensures that compare Damage was called
                _specialCaseResolverMock.Received(1).Resolve(monster1, monster2);

            }
    }
}
