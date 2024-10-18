using MTCG.Auth;
using MTCG.BusinessLayer.Model.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.BusinessLayer.Controller
{
    internal class ScoreboardController
    {

        private static ScoreboardController _instance;
        public static ScoreboardController Instance => _instance ??= new ScoreboardController();

        private ScoreboardController()
        {
        }

        public List<UserStatsDTO> GetScoreboardDTOs()
        {
            var allUsersDTOs = AuthenticationController.Instance.GetUsers().Select(user => new UserStatsDTO()
            {
                Username = user.GetName(),
                Wins = user.Stats.Wins,
                Losses = user.Stats.Losses,
                EloPoints = user.Stats.Elo.EloScore,
                EloName = user.Stats.Elo.GetEloName()
            }).ToList();

            allUsersDTOs.Sort((user1, user2) => user1.EloPoints.CompareTo(user2.EloPoints));

            return allUsersDTOs;
        }
    }
}
