using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.DataAccessLayer
{
    internal class UserTokenRepository : IRepository<UserToken>
    {

        private static UserTokenRepository? _instance;

        public static UserTokenRepository Instance => _instance ??= new UserTokenRepository();

        private UserTokenRepository()
        {
        }

        public async Task<int> Add(UserToken entity)
        {
            await using var conn = ConnectionController.CreateConnection();
            await conn.OpenAsync();
            await using var command = conn.CreateCommand();

            command.CommandText = "INSERT INTO \"UserToken\" (\"userID\", \"authToken\") " +
                                  "VALUES (@userID, @authToken) RETURNING id";
            AddParameterWithValue(command, "userID", DbType.Int32, entity.UserID);
            AddParameterWithValue(command, "authToken", DbType.String, entity.Token.Value);
            
            return (int)(await command.ExecuteScalarAsync() ?? -1);

        }

        public async Task<int> Delete(UserToken entity)
        {
            await using var conn = ConnectionController.CreateConnection();
            await conn.OpenAsync();
            await using var command = conn.CreateCommand();

            command.CommandText = "DELETE FROM \"UserToken\" WHERE \"authToken\" = @authToken";
            AddParameterWithValue(command, "authToken", DbType.String, entity.Token.Value);

            return (int)(await command.ExecuteNonQueryAsync());
        }

        public IEnumerable<UserToken> GetAll()
        {
            throw new NotImplementedException();
        }

        public UserToken GetById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<int> GetByAuthToken(string authToken)
        {
            await using var conn = ConnectionController.CreateConnection();
            await conn.OpenAsync();
            await using var command = conn.CreateCommand();

            command.CommandText = "SELECT \"userID\" FROM \"UserToken\" WHERE \"authToken\" = @authToken";
            AddParameterWithValue(command, "authToken", DbType.String, authToken);

            await using var reader = await command.ExecuteReaderAsync();

            var userID = -1;

            if (await reader.ReadAsync())
            {
                userID = reader.GetInt32("userID");
            }

            return userID;
        }

        public void Update(UserToken entity)
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
