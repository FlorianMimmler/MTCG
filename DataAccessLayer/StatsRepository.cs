using MTCG.BusinessLayer.Model.User;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.DataAccessLayer
{
    internal class StatsRepository : IRepository<Stats>
    {
        private string ConnectionString;

        private static StatsRepository _instance;

        public static StatsRepository Instance => _instance ??= new StatsRepository();

        private StatsRepository()
        {
            ConnectionString = "Host=localhost;Username=admin;Password=password;Database=MTCG";
        }

        public async Task<int> Add(Stats entity)
        {
            await using var conn = new NpgsqlConnection(ConnectionString);
            await conn.OpenAsync();
            using var command = conn.CreateCommand();

            command.CommandText = "INSERT INTO \"Stats\" default values RETURNING id";

            return (int) (await command.ExecuteScalarAsync() ?? -1);
        }

        public void Delete(Stats entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Stats> GetAll()
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
