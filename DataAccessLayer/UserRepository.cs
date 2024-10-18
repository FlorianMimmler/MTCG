using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;

namespace MTCG.DataAccessLayer
{
    internal class UserRepository : IRepository<User>
    {

        private readonly string ConnectionString;

        public UserRepository _instance;

        public UserRepository Instance => _instance ??= new UserRepository();


        private UserRepository()
        {
            ConnectionString = "Host=localhost;Username=admin;Password=password;Database=MTCG";
        }

        public void Add(User entity)
        {

            using IDbConnection connection = new NpgsqlConnection(ConnectionString);
            using var command = connection.CreateCommand();
            connection.Open();

            command.CommandText = "INSERT INTO User (username, password, salt, statsID) " +
                                  "VALUES (@username, @password, @salt, @statsID) RETURNING id";
            AddParameterWithValue(command, "username", DbType.String, entity.GetName());
            AddParameterWithValue(command, "password", DbType.Int32, entity.Credentials.Password);
            AddParameterWithValue(command, "salt", DbType.String, entity.Credentials.Salt);
            AddParameterWithValue(command, "statsID", DbType.String, entity.Stats.Id);
            entity.Id = (int)(command.ExecuteScalar() ?? 0);
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
