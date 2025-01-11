using MTCG.BusinessLayer.Model.User;

namespace MTCG_uTests.UserFeaturesTests
{
    internal class EloTests
    {

        private Elo _elo;

        [SetUp]
        public void Setup()
        {
            _elo = new Elo();
        }

        [Test]
        public void Increase_StandardEloscore_UpdateEloscore()
        {
            //Act
            _elo.Increase();

            //Assert
            Assert.That(_elo.EloScore, Is.EqualTo(95));

        }

        [Test]
        public void Increase_CustomEloscore_UpdateEloscore()
        {

            //Act
            _elo.Increase(10);

            //Assert
            Assert.That(_elo.EloScore, Is.EqualTo(100));

        }

        [Test]
        public void Decrease_CustomEloscore_UpdateEloscore()
        {
            //Act
            _elo.Decrease(10);

            //Assert
            Assert.That(_elo.EloScore, Is.EqualTo(80));

        }

        [Test]
        public void Decrease_CustomEloscoreResultMinusScore_UpdateEloscoreToZero()
        {

            //Act
            _elo.Decrease(10000);

            //Assert
            Assert.That(_elo.EloScore, Is.EqualTo(0));

        }

        [TestCase(50, "Bronce")]
        [TestCase(75, "Silber")]
        [TestCase(110, "Gold")]
        [TestCase(150, "Platinum")]
        [TestCase(190, "Diamond")]
        [TestCase(230, "Master")]
        [TestCase(300, "Grandmaster")]
        public void GetEloName_ReturnsCorrectEloName(int score, string expectedName)
        {
            // Given
            _elo.EloScore = score;

            // Act
            var result = _elo.GetEloName();

            // Assert
            Assert.That(result, Is.EqualTo(expectedName));
        }

    }
}
