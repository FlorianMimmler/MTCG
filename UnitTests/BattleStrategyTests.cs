using MTCG.BusinessLayer.Interface;
using MTCG.BusinessLayer.Model.BattleStrategy;
using MTCG.BusinessLayer.Model.Card;
using MTCG.BusinessLayer.Model.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG_uTests
{
    internal class BattleStrategyTests
    {


        [SetUp]
        public void Setup()
        {
        }

        [Test]
        [TestCase(10.5, 20, BattleResult.Player2Wins)]
        [TestCase(20.5, 10, BattleResult.Player1Wins)]
        [TestCase(10.1, 10.1, BattleResult.Tie)]
        public void CompareDamage_SpecificParameters_ShouldReturnSpecificResult(double damage1, double damage2, BattleResult expectedResult)
        {
            //Arrange
            var battleStrategy = new MixedBattleStrategy();

            //Act
            var result = battleStrategy.CompareDamage(damage1, damage2);

            //Assert
            Assert.That(result, Is.EqualTo(expectedResult));

        }

        [Test]
        public void Execute_KnightVSWaterSpell_WaterSpellAsPlayer2ShouldWin()
        {
            //Arrange
            var mixedBattle = new MixedBattleStrategy();
            var knight = new MonsterCard(200, ElementType.NORMAL, MonsterType.Knight);
            var waterSpell = new SpellCard(1, ElementType.WATER);

            //Act
            var result = mixedBattle.Execute(knight, waterSpell);

            //Assert
            Assert.That(result, Is.EqualTo(BattleResult.Player2Wins));
        }

        [Test]
        [TestCase(ElementType.WATER)]
        [TestCase(ElementType.FIRE)]
        [TestCase(ElementType.NORMAL)]
        public void Execute_KrakenVSAnySpell_KrakenAsPlayer1ShouldWin(ElementType spellType)
        {
            //Arrange
            var mixedBattle = new MixedBattleStrategy();
            var kraken = new MonsterCard(1, ElementType.NORMAL, MonsterType.Kraken);
            var spell = new SpellCard(200, spellType);

            //Act
            var result = mixedBattle.Execute(kraken, spell);

            //Assert
            Assert.That(result, Is.EqualTo(BattleResult.Player1Wins));
        }

        [Test]
        [TestCase(ElementType.FIRE, ElementType.NORMAL, 2.0)]
        [TestCase(ElementType.FIRE, ElementType.WATER, 0.5)]
        [TestCase(ElementType.WATER, ElementType.FIRE, 2.0)]
        [TestCase(ElementType.WATER, ElementType.NORMAL, 0.5)]
        [TestCase(ElementType.NORMAL, ElementType.WATER, 2.0)]
        [TestCase(ElementType.NORMAL, ElementType.FIRE, 0.5)]
        public void GetEffectiveness_DefinedValues_ShouldReturnExpectedResult(ElementType attacker, ElementType defender, double expected)
        {
            // Act
            var result = ElementEffectiveness.GetEffectiveness(attacker, defender);

            // Assert
            Assert.That(result, Is.EqualTo(expected), $"Effectiveness from {attacker} to {defender} should be {expected}.");
        }

        [Test]
        [TestCase(MonsterType.Goblin, MonsterType.Dragon, BattleResult.Player2Wins)]
        [TestCase(MonsterType.Wizzard, MonsterType.Ork, BattleResult.Player1Wins)]
        [TestCase(MonsterType.Elve, MonsterType.Dragon, BattleResult.Tie)]
        [TestCase(MonsterType.Elve, MonsterType.Dragon, BattleResult.Player1Wins, ElementType.FIRE)]
        public void Resolve_SpecialCase_ShouldReturnExpectedResult(MonsterType type1, MonsterType type2, BattleResult expected, ElementType elementType = ElementType.NORMAL)
        {
            // Arrange
            var resolver = new SpecialCaseResolver();
            var card1 = new MonsterCard(50, elementType, type1);
            var card2 = new MonsterCard(50, ElementType.NORMAL, type2);

            // Act
            var result = resolver.Resolve(card1, card2);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

    }
}
