using MTCG.BusinessLayer.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.BusinessLayer.Model.Achievements;

namespace MTCG.DataAccessLayer
{

    internal class UserAchievementRepository : IRepository<UserAchievement>

    {

        private static UserAchievementRepository? _instance;

        public static UserAchievementRepository Instance => _instance ??= new UserAchievementRepository();

        public async Task<int> Add(UserAchievement entity)
        {
            await using var conn = ConnectionController.CreateConnection();
            await conn.OpenAsync();
            await using var command = conn.CreateCommand();

            command.CommandText = """
                                   INSERT INTO "UserAchievement" ("user", achievement)
                                   VALUES(@userId, @achievementId) RETURNING id
                                  """;

            ConnectionController.AddParameterWithValue(command, "userId", DbType.Int32, entity.UserId);
            ConnectionController.AddParameterWithValue(command, "achievementId", DbType.Int32, entity.Achievement.Id);

            return (int)(await command.ExecuteScalarAsync() ?? 0);
        }

        public Task<int> Delete(UserAchievement entity)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<UserAchievement>?> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<UserAchievement?> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update(UserAchievement entity)
        {
            throw new NotImplementedException();
        }
    }
}
