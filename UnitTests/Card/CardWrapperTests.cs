

using MTCG.BusinessLayer.Controller;
using MTCG.BusinessLayer.Model.CardWrapper;

namespace MTCG_uTests
{
    internal class CardWrapperTests
    {

        [SetUp]
        public void SetUp()
        {
            CardController.Instance = null;
        }

        [Test]
        public void PackageConstructor_Default_PackageShouldContain5Cards()
        {
            //Act
            var package = new Package();

            //Assert
            Assert.That(package.Cards.Count, Is.EqualTo(5));

        }

    }
}
