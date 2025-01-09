using NSubstitute;
using MTCG.BusinessLayer.Model.User;
using MTCG.DataAccessLayer;
using MTCG.Auth;

namespace MTCG_uTests
{
    public class AuthenticationControllerTests
    {
        private IUserRepository _userRepositoryMock;
        private IUserTokenRepository _userTokenRepositoryMock;
        private IStatsRepository _statsRepositoryMock;
        private AuthenticationController _controller;

        [SetUp]
        public void SetUp()
        {
            _userRepositoryMock = Substitute.For<IUserRepository>();
            _userTokenRepositoryMock = Substitute.For<IUserTokenRepository>();
            _statsRepositoryMock = Substitute.For<IStatsRepository>();

            // Set the repository instances
            UserRepository.Instance = _userRepositoryMock;
            UserTokenRepository.Instance = _userTokenRepositoryMock;
            StatsRepository.Instance = _statsRepositoryMock;

            _controller = AuthenticationController.Instance;
        }

        [Test]
        public async Task Login_InvalidUser_ShouldReturnInvalidAuthToken()
        {
            // Arrange
            var creds = new Credentials("invalidUser", "password");
            _userRepositoryMock.GetByUsername(creds.Username).Returns(Task.FromResult((User)null));

            // Act
            var result = await _controller.Login(creds);

            // Assert
            Assert.That(result.Valid, Is.False);
        }

        [Test]
        public async Task Login_ValidUserInvalidPassword_ShouldReturnInvalidAuthToken()
        {
            // Arrange
            var creds = new Credentials("validUser", "wrongPassword");
            var userCreds = new Credentials();
            userCreds.SetPassword("hashedPassword");
            userCreds.SetSalt("salt");
            var user = new User { Id = 1, Credentials = userCreds };

            _userRepositoryMock.GetByUsername(creds.Username).Returns(Task.FromResult(user));

            // Act
            var result = await _controller.Login(creds);

            // Assert
            Assert.That(result.Valid, Is.False);
        }

        [Test]
        public async Task Login_ValidUserValidPassword_ShouldReturnValidAuthToken()
        {
            // Arrange
            var creds = new Credentials("validUser", "correctPassword");
            creds.SetPassword("correctPassword");
            var userCreds = new Credentials("validUser", "correctPassword");
            userCreds.SetSalt("salt");
            userCreds.SetPasswordAndHash("correctPassword");
            var user = new User { Id = 1, Credentials = userCreds };

            _userRepositoryMock.GetByUsername(creds.Username).Returns(Task.FromResult(user));
            _userTokenRepositoryMock.Add(Arg.Any<UserToken>()).Returns(Task.FromResult(1));

            // Act
            var result = await _controller.Login(creds);

            // Assert
            Assert.That(result.Valid, Is.True);
        }

        [Test]
        public async Task Signup_UserAlreadyExists_ShouldReturnNegativeOne()
        {
            // Arrange
            var creds = new Credentials("existingUser", "password");
            _userRepositoryMock.GetByUsername(creds.Username).Returns(Task.FromResult(new User()));

            // Act
            var result = await _controller.Signup(creds);

            // Assert
            Assert.That(result, Is.EqualTo(-1));
        }

        [Test]
        public async Task Signup_NewUser_ShouldReturnPositiveId()
        {
            // Arrange
            var creds = new Credentials("newUser", "password");
            _userRepositoryMock.GetByUsername(creds.Username).Returns(Task.FromResult((User)null));
            _statsRepositoryMock.Add(Arg.Any<Stats>()).Returns(Task.FromResult(1));
            _userRepositoryMock.Add(Arg.Any<User>()).Returns(Task.FromResult(1));

            // Act
            var result = await _controller.Signup(creds);

            // Assert
            Assert.That(result, Is.GreaterThan(0));
        }

        [Test]
        public async Task Logout_ValidAuthToken_ShouldReturnTrue()
        {
            // Arrange
            var authToken = "validAuthToken";
            _userTokenRepositoryMock.Delete(Arg.Any<UserToken>()).Returns(Task.FromResult(1));

            // Act
            var result = await _controller.Logout(authToken);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task Logout_InvalidAuthToken_ShouldReturnFalse()
        {
            // Arrange
            var authToken = "invalidAuthToken";
            _userTokenRepositoryMock.Delete(Arg.Any<UserToken>()).Returns(Task.FromResult(0));

            // Act
            var result = await _controller.Logout(authToken);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task IsAuthorized_ValidAuthToken_ShouldReturnTrue()
        {
            // Arrange
            var authToken = "validAuthToken";
            _userTokenRepositoryMock.GetByAuthToken(authToken).Returns(Task.FromResult(1));

            // Act
            var result = await _controller.IsAuthorized(authToken);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task IsAuthorized_InvalidAuthToken_ShouldReturnFalse()
        {
            // Arrange
            var authToken = "invalidAuthToken";
            _userTokenRepositoryMock.GetByAuthToken(authToken).Returns(Task.FromResult(-1));

            // Act
            var result = await _controller.IsAuthorized(authToken);

            // Assert
            Assert.That(result, Is.False);
        }
    }
}
