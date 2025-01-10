using MTCG.BusinessLayer.Controller;
using MTCG.BusinessLayer.Model.Achievements;
using MTCG.DataAccessLayer;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG_uTests
{
    internal class AchievementTests
    {

        [Test]
        [TestCase("wins", AchievementTypes.Wins)]
        [TestCase("elo", AchievementTypes.Elo)]
        [TestCase("coins", AchievementTypes.Coins)]
        public void StringAchievementToEnum_ValidInputs_ShouldReturnCorrectEnum(string input, AchievementTypes expected)
        {
            // Act
            var result = Achievement.StringAchievementToEnum(input);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Achievement_SetProperties_ShouldReturnCorrectValues()
        {
            // Arrange
            var achievement = new Achievement
            {
                Id = 1,
                Name = "10 total wins",
                Type = AchievementTypes.Wins,
                Value = 10,
                RewardType = AchievementTypes.Coins,
                RewardValue = 100
            };

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(achievement.Id, Is.EqualTo(1));
                Assert.That(achievement.Name, Is.EqualTo("10 total wins"));
                Assert.That(achievement.Type, Is.EqualTo(AchievementTypes.Wins));
                Assert.That(achievement.Value, Is.EqualTo(10));
                Assert.That(achievement.RewardType, Is.EqualTo(AchievementTypes.Coins));
                Assert.That(achievement.RewardValue, Is.EqualTo(100));
            });
        }
    }
}
