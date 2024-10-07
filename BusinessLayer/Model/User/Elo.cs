
namespace MTCG.BusinessLayer.Model.User
{
    internal class Elo
    {

        public int EloScore { get; set; } = 100;

        public void Increase()
        {
            this.EloScore += 5;
        }

        public void Decrease()
        {
            this.EloScore -= 3;
        }

    }
}
