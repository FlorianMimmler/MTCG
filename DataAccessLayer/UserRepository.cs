using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;

namespace MTCG.DataAccessLayer
{
    internal class UserRepository : IRepository<User>
    {

        private readonly string ConnectionString;

        public static UserRepository _instance;

        public static UserRepository Instance => _instance ??= new UserRepository();


        private UserRepository()
        {
            ConnectionString = "Host=localhost;Port=5432;Username=admin;Password=password;Database=MTCG";
        }

        public async Task<int> Add(User entity)
        {
            await using var conn = new NpgsqlConnection(ConnectionString);
            await conn.OpenAsync();
            using var command = conn.CreateCommand();

            command.CommandText = "INSERT INTO \"User\" (username, password, salt, \"statsID\") " +
                                  "VALUES (@username, @password, @salt, @statsID) RETURNING id";
            AddParameterWithValue(command, "username", DbType.String, entity.GetName());
            AddParameterWithValue(command, "password", DbType.String, entity.Credentials.Password);
            AddParameterWithValue(command, "salt", DbType.String, entity.Credentials.Salt);
            AddParameterWithValue(command, "statsID", DbType.Int32, entity.Stats.Id);
            return (int)(await command.ExecuteScalarAsync() ?? 0);
    
        }

        public void Delete(User entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<User> GetAll()
        {
            throw new NotImplementedException();
        }

        public User GetById(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(User entity)
        {
            throw new NotImplementedException();
        }

        public static void AddParameterWithValue(IDbCommand command, string parameterName, DbType type, object value)
        {
            var parameter = command.CreateParameter();
            parameter.DbType = type;
            parameter.ParameterName = parameterName;
            parameter.Value = value ?? DBNull.Value;
            command.Parameters.Add(parameter);
        }

    }
}
