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
    internal class AchievementControllerTests
    {
        private AchievementController _controller;
        private IAchievementRepository _achievementRepositoryMock;

        [SetUp]
        public void SetUp()
        {
            _achievementRepositoryMock = Substitute.For<IAchievementRepository>();

            // Override the singleton instance with the mock
            AchievementRepository.Instance = _achievementRepositoryMock;

            AchievementController.Instance = null;
            _controller = AchievementController.Instance;
        }

        [Test]
        public void GetAchievements_NoAchievementsLoaded_ShouldReturnEmptyList()
        {
            // Arrange
            _achievementRepositoryMock.GetAll().Returns(new List<Achievement>());

            // Act
            var result = _controller.GetAchievements();

            // Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void GetAchievements_AchievementsLoaded_ShouldReturnAchievements()
        {
            // Arrange
            var achievements = new List<Achievement>
            {
                new Achievement { Id = 1, Name = "First Win", Type = AchievementTypes.Wins, Value = 1 },
                new Achievement { Id = 2, Name = "Elo Master", Type = AchievementTypes.Elo, Value = 1000 }
            };

            _achievementRepositoryMock.GetAll().Returns(achievements);

            // Trigger LoadAchievements indirectly by creating the controller instance
            AchievementController.Instance = null; // Reset singleton for testing
            _controller = AchievementController.Instance;

            // Act
            var result = _controller.GetAchievements();

            // Assert
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result.First().Name, Is.EqualTo("First Win"));
        }
    }
}
