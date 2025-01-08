using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.BusinessLayer.Model.Achievements;
using MTCG.DataAccessLayer;

namespace MTCG.BusinessLayer.Controller
{
    internal class AchievementController
    {

        private static AchievementController _instance;

        public static AchievementController Instance => _instance ??= new AchievementController();

        private AchievementController()
        {
            LoadAchievements();
        }

        private async void LoadAchievements()
        {
            _achievementList = await AchievementRepository.Instance.GetAll();
        }

        private IEnumerable<Achievement>? _achievementList;

        public Achievement? GetAchievementById(int id)
        {
            return _achievementList?.First(achievement => achievement.Id == id);
        }

        public List<Achievement>? GetAchievements()
        {
            return _achievementList?.ToList();
        }

    }
}
