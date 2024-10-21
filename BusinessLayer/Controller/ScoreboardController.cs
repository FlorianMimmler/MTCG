using MTCG.BusinessLayer.Model.User;
using MTCG.DataAccessLayer;

namespace MTCG.BusinessLayer.Controller
{
    internal class ScoreboardController
    {

        private static ScoreboardController? _instance;
        public static ScoreboardController Instance => _instance ??= new ScoreboardController();

        private ScoreboardController()
        {
        }

        public async Task<List<UserStatsDTO>?> GetScoreboardDTOs()
        {
            var allUsers = await UserRepository.Instance.GetAll();

            if (allUsers == null) return null;

            var allUsersDTOs = allUsers.Select(user => new UserStatsDTO()
            {
                Username = user.GetName(),
                Wins = user.Stats.Wins,
                Losses = user.Stats.Losses,
                EloPoints = user.Stats.Elo.EloScore,
                EloName = user.Stats.Elo.GetEloName()
            }).ToList();

            allUsersDTOs.Sort((user1, user2) => user1.EloPoints.CompareTo(user2.EloPoints));
            allUsersDTOs.Reverse();

            return allUsersDTOs;
        }
    }
}
