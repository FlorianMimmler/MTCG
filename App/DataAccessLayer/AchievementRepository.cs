using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.Auth;
using MTCG.BusinessLayer.Controller;
using MTCG.BusinessLayer.Model.Achievements;
using MTCG.BusinessLayer.Model.User;

namespace MTCG.DataAccessLayer
{
    public class AchievementRepository : IAchievementRepository
    {

        private static IAchievementRepository? _instance;

        public static IAchievementRepository Instance
        {
            get => _instance ??= new AchievementRepository();
            set => _instance = value;
        } 

        public Task<int> Add(Achievement entity)
        {
            throw new NotImplementedException();
        }

        public Task<int> Delete(Achievement entity)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Achievement>?> GetAll()
        {
            await using var command = await ConnectionController.GetCommandConnection();

            if (command == null || command.Connection == null)
            {
                return null;
            }

            command.CommandText = """
                                  SELECT a.id, a.name, a.type, a.value, a.rewardtype, a.rewardvalue
                                  FROM "Achievement" a
                                  """;

            await using var reader = await command.ExecuteReaderAsync();

            var achievements = new List<Achievement>();
            while (await reader.ReadAsync())
            {
                achievements.Add(new Achievement
                {
                    Id = reader.GetInt32("id"),
                    Name = reader.GetString("name"),
                    Type = Achievement.StringAchievementToEnum(reader.GetString("type")),
                    Value = reader.GetInt32("value"),
                    RewardType = Achievement.StringAchievementToEnum(reader.GetString("rewardtype")),
                    RewardValue = reader.GetInt32("rewardvalue"),
                });
            }

            await command.Connection.CloseAsync();

            return achievements;
        }

        public Task<Achievement?> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Achievement>?> GetAchievementsByUser(int userId)
        {
            await using var command = await ConnectionController.GetCommandConnection();

            if (command == null || command.Connection == null)
            {
                return null;
            }

            command.CommandText = """
                                  SELECT a.id, a.name, a.type, a.value, a.rewardtype, a.rewardvalue
                                  FROM "Achievement" a
                                  INNER JOIN "UserAchievement" ua ON a.id = ua.achievement
                                  WHERE ua."user" = @userId
                                  """;

            ConnectionController.AddParameterWithValue(command, "userId", DbType.Int32, userId);

            await using var reader = await command.ExecuteReaderAsync();

            var achievements = new List<Achievement>();
            while (await reader.ReadAsync())
            {
                achievements.Add(new Achievement
                {
                    Id = reader.GetInt32("id"),
                    Name = reader.GetString("name"),
                    Type = Achievement.StringAchievementToEnum(reader.GetString("type")),
                    Value = reader.GetInt32("value"),
                    RewardType = Achievement.StringAchievementToEnum(reader.GetString("rewardtype")),
                    RewardValue = reader.GetInt32("rewardvalue"),
                });
            }
            await command.Connection.CloseAsync();
            return achievements;
        }


        public Task<bool> Update(Achievement entity)
        {
            throw new NotImplementedException();
        }
    }
}
