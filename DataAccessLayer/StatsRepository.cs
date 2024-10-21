using MTCG.BusinessLayer.Model.User;

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

        public Stats GetById(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(Stats entity)
        {
            throw new NotImplementedException();
        }
    }
}
