using MTCG.Auth;
using MTCG.BusinessLayer.Model.User;

namespace MTCG_uTests.UserFeaturesTests
{
    public class UserTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GetName_SpecifiedName_ShouldReturnName()
        {
            //Arrange
            const string specifiedName = "testUser";
            var user = new User(new Credentials(specifiedName, "test"));

            //Act
            var username = user.GetName();

            //Assert
            Assert.That(username, Is.EqualTo(specifiedName));
        }

        [Test]
        public async Task BuyPackage_WithoutEnoughCoins_ShouldFailWithCode2()
        {
            //Arrange
            var user = new User(new Credentials("testUser", "test"))
            {
                Coins = 0
            };

            //Act
            var result = await user.BuyPackage();

            //Assert
            Assert.That(result, Is.EqualTo(2));

        }
    }
}