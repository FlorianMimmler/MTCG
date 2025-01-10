using MTCG.BusinessLayer.Model.Achievements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.DataAccessLayer
{
    public interface IAchievementRepository : IRepository<Achievement>
    {

        public Task<List<Achievement>?> GetAchievementsByUser(int userId);
    }
}
