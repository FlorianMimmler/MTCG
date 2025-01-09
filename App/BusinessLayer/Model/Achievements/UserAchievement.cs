using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.BusinessLayer.Model.Achievements
{
    internal class UserAchievement(int userId, Achievement achievement)
    {
        public int UserId { get; set; } = userId;
        public Achievement Achievement { get; set; } = achievement;

    }
}
