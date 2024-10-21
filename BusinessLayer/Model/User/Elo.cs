
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

        public int EloScore { get; set; } = 100;

        public void Increase()
        {
            this.EloScore += 5;
        }

        public void Decrease()
        {
            this.EloScore -= 3;
        }

        public string GetEloName()
        {
            if (EloScore < 80)
            {
                return "Bronce";
            }
            if (EloScore >= 80 && EloScore < 105)
            {
                return "Silber";
            }
            if (EloScore >= 105 && EloScore < 125)
            {
                return "Gold";
            }
            if (EloScore >= 125 && EloScore < 150)
            {
                return "Platinum";
            }
            if (EloScore >= 150 && EloScore < 175)
            {
                return "Diamond";
            }
            if (EloScore >= 175 && EloScore < 200)
            {
                return "Master";
            }
            return "Grandmaster";

        }

    }
}
