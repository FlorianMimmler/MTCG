using MTCG.Auth;
using MTCG.BusinessLayer.Model.User;
using MTCG.DataAccessLayer;
using MTCG;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.BusinessLayer.Controller;

namespace MTCG_uTests
{
    internal class ScoreboardControllerTests
    {
        private ScoreboardController _controller;
        private IUserRepository _userRepositoryMock;

        [SetUp]
        public void SetUp()
        {
            _userRepositoryMock = Substitute.For<IUserRepository>();

            // Override the singleton instance with the mock
            UserRepository.Instance = _userRepositoryMock;

            _controller = ScoreboardController.Instance;
        }

        [Test]
        public async Task GetScoreboardDTOs_UsersExist_ShouldReturnSortedDTOs()
        {
            // Arrange
            var users = new List<User>
            {
                new User { Stats = new Stats { Wins = 10, Losses = 5, Elo = new Elo { EloScore = 1200 } }, Credentials = new Credentials { Username = "Alice" } },
                new User { Stats = new Stats { Wins = 15, Losses = 3, Elo = new Elo { EloScore = 1300 } }, Credentials = new Credentials { Username = "Bob" } },
                new User { Stats = new Stats { Wins = 5, Losses = 10, Elo = new Elo { EloScore = 1000 } }, Credentials = new Credentials { Username = "Charlie" } }
            };

            _userRepositoryMock.GetAll().Returns(users);

            // Act
            var result = await _controller.GetScoreboardDTOs();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(3));
            Assert.That(result[0].Username, Is.EqualTo("Bob")); // Highest Elo
            Assert.That(result[1].Username, Is.EqualTo("Alice")); // Second highest Elo
            Assert.That(result[2].Username, Is.EqualTo("Charlie")); // Lowest Elo
        }

        [Test]
        public async Task GetScoreboardDTOs_NoUsersExist_ShouldReturnNull()
        {
            // Arrange
            _userRepositoryMock.GetAll().Returns((IEnumerable<User>?)null);

            // Act
            var result = await _controller.GetScoreboardDTOs();

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetScoreboardDTOs_EmptyRepository_ShouldReturnEmptyList()
        {
            // Arrange
            _userRepositoryMock.GetAll().Returns(new List<User>());

            // Act
            var result = await _controller.GetScoreboardDTOs();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
        }
    }
}
