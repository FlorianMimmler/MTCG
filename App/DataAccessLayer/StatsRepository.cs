using MTCG.BusinessLayer.Model.User;
using System.Data;

namespace MTCG.DataAccessLayer
{
    internal class StatsRepository : IRepository<Stats>
    {

        private static StatsRepository? _instance;

        public static StatsRepository Instance => _instance ??= new StatsRepository();

        private StatsRepository()
        {
        }

        public async Task<int> Add(Stats entity)
        {
            await using var conn = ConnectionController.CreateConnection();
            await conn.OpenAsync();
            await using var command = conn.CreateCommand();

            command.CommandText = "INSERT INTO \"Stats\" default values RETURNING id";

            return (int) (await command.ExecuteScalarAsync() ?? -1);
        }

        public Task<int> Delete(Stats entity)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Stats>?> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<Stats?> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Update(Stats entity)
        {
            await using var conn = ConnectionController.CreateConnection();
            await conn.OpenAsync();
            await using var command = conn.CreateCommand();

            command.CommandText = "UPDATE \"Stats\" SET wins = @wins, losses = @losses, eloscore = @eloscore WHERE id = @id";

            ConnectionController.AddParameterWithValue(command, "wins", DbType.Int32, entity.Wins);
            ConnectionController.AddParameterWithValue(command, "losses", DbType.Int32, entity.Losses);
            ConnectionController.AddParameterWithValue(command, "eloscore", DbType.Int32, entity.Elo.EloScore);
            ConnectionController.AddParameterWithValue(command, "id", DbType.Int32, entity.Id);

            return await command.ExecuteNonQueryAsync() == 1;
        }
    }
}
