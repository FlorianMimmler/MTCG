using MTCG.BusinessLayer.Model.User;

namespace MTCG_uTests.UserFeaturesTests
{
    internal class StatsTests
    {

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void AddWin_AddsOneWin_ShouldUpdateStats()
        {
            //Arrange
            var stats = new Stats();

            //Act
            stats.AddWin();

            //Assert
            Assert.That(stats.Wins, Is.EqualTo(1));

        }

        [Test]
        public void AddLoss_AddsOneLoss_ShouldUpdateStats()
        {
            //Arrange
            var stats = new Stats();

            //Act
            stats.AddLoss();

            //Assert
            Assert.That(stats.Losses, Is.EqualTo(1));

        }
    }
}
