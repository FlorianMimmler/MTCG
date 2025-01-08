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
            //Given
            var stats = new Stats();

            //When
            stats.AddWin();

            //Then
            Assert.That(stats.Wins, Is.EqualTo(1));

        }

        [Test]
        public void AddLoss_AddsOneLoss_ShouldUpdateStats()
        {
            //Given
            var stats = new Stats();

            //When
            stats.AddLoss();

            //Then
            Assert.That(stats.Losses, Is.EqualTo(1));

        }
    }
}
