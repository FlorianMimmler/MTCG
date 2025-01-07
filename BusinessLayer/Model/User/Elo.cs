
namespace MTCG.BusinessLayer.Model.User
{
    internal enum EloName
    {
        Bronce,
        Silber,
        Gold,
        Platin,
        Diamond,
        Master,
        GrandMaster
    }
    internal class Elo
    {

        public int EloScore { get; set; } = 90;

        public void Increase(int points = 5)
        {
            this.EloScore += points;
        }

        public void Decrease(int points = 3)
        {
            this.EloScore -= points;
            if (EloScore < 0)
            {
                EloScore = 0;
            }
        }

        public string GetEloName()
        {
            if (EloScore < 80)
            {
                return "Bronce";
            }
            if (EloScore >= 70 && EloScore < 110)
            {
                return "Silber";
            }
            if (EloScore >= 110 && EloScore < 150)
            {
                return "Gold";
            }
            if (EloScore >= 150 && EloScore < 190)
            {
                return "Platinum";
            }
            if (EloScore >= 190 && EloScore < 230)
            {
                return "Diamond";
            }
            if (EloScore >= 230 && EloScore < 300)
            {
                return "Master";
            }
            return "Grandmaster";

        }

    }
}
