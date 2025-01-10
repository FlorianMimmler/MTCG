using MTCG.Auth;

namespace MTCG_uTests
{
    internal class AuthTokenTests
    {

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void AuthTokenConstructor_Default_ShouldBeInvalidAndEmpty()
        {
            // Act
            var authToken = new AuthToken();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(authToken.Valid, Is.False, "Default AuthToken should be invalid.");
                Assert.That(authToken.Value, Is.Empty, "Default AuthToken value should be empty.");
            });
        }

        [Test]
        public void AuthTokenConstructor_WithTrue_ShouldBeValidAndHaveValue()
        {
            // Act
            var authToken = new AuthToken(true);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(authToken.Valid, Is.True, "AuthToken should be valid when created with true.");
                Assert.That(authToken.Value, Is.Not.Empty, "AuthToken should have a value when created with true.");
            });
        }

        [Test]
        public void Reset_Default_ShouldBeEmptyAndNotValid()
        {
            //Arrange
            var authToken = new AuthToken(true);

            //Act
            authToken.Reset();

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(authToken.Valid, Is.False, "AuthToken should be not valid after reset.");
                Assert.That(authToken.Value, Is.Empty, "AuthToken should be empty after reset.");
            });

        }

    }
}
