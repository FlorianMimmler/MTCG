using MTCG.BusinessLayer.Model.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG_uTests
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
