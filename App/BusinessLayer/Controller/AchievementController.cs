using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.BusinessLayer.Model.Achievements;
using MTCG.DataAccessLayer;

namespace MTCG.BusinessLayer.Controller
{
    public class AchievementController
    {

        private static AchievementController? _instance;

        public static AchievementController Instance
        {
            get => _instance ??= new AchievementController();
            set => _instance = value;
        }

        private AchievementController()
        {
            LoadAchievements();
        }

        private IEnumerable<Achievement>? _achievementList;

        private async void LoadAchievements()
        {
            _achievementList = await AchievementRepository.Instance.GetAll();
        }

        public List<Achievement> GetAchievements()
        {
            return _achievementList?.ToList() ?? [];
        }

    }
}
