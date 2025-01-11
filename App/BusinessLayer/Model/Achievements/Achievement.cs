using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.BusinessLayer.Model.Achievements
{
    public class Achievement
    {
        public int Id { get; set; }

        public string Name { get; set; } = "";
        public AchievementTypes Type { get; set; }
        public int Value { get; set; }
        public AchievementTypes RewardType { get; set; }
        public int RewardValue { get; set; }

        public static AchievementTypes StringAchievementToEnum(string type)
        {
            return type switch
            {
                "wins" => AchievementTypes.Wins,
                "elo" => AchievementTypes.Elo,
                "coins" => AchievementTypes.Coins,
                _ => AchievementTypes.Wins,
            };
        }
    }

    public enum AchievementTypes
    {
        Wins,
        Elo,
        Coins
    }
}
