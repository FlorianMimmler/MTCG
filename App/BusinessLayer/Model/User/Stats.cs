namespace MTCG.BusinessLayer.Model.User
{
    public class Stats
    {
        public int Id { get; set; }
        public int Wins { get; set; } = 0;
        public int Losses { get; set; } = 0;

        public Elo Elo = new();

        public void AddWin()
        {
            Wins++;
        }

        public void AddLoss()
        {
            Losses++;
        }
    }
}
