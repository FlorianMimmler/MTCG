using MTCG.Auth;
using MTCG.BusinessLayer.Model.User;
using System.Data;

namespace MTCG.DataAccessLayer
{
    public class UserTokenRepository : IUserTokenRepository
    {

        private static IUserTokenRepository? _instance;

        public static IUserTokenRepository Instance
        {
            get => _instance ??= new UserTokenRepository();
            set => _instance = value;
        } 

        private UserTokenRepository()
        {
        }

        public async Task<int> Add(UserToken entity)
        {
            await using var command = await ConnectionController.GetCommandConnection();

            if (command == null || command.Connection == null)
            {
                return -1;
            }

            command.CommandText = "INSERT INTO \"UserToken\" (\"userID\", \"authToken\") " +
                                  "VALUES (@userID, @authToken) RETURNING id";
            ConnectionController.AddParameterWithValue(command, "userID", DbType.Int32, entity.UserID);
            ConnectionController.AddParameterWithValue(command, "authToken", DbType.String, entity.Token.Value);
            try { 
                return (int)(await command.ExecuteScalarAsync() ?? -1);
            }
            finally
            {
                await command.Connection.CloseAsync(); // Ensure connection is closed
            }

        }

        public async Task<int> Delete(UserToken entity)
        {
            await using var command = await ConnectionController.GetCommandConnection();

            if (command == null || command.Connection == null)
            {
                return -1;
            }

            command.CommandText = "DELETE FROM \"UserToken\" WHERE \"authToken\" = @authToken";
            ConnectionController.AddParameterWithValue(command, "authToken", DbType.String, entity.Token.Value);
            try { 
                return (int)(await command.ExecuteNonQueryAsync());
            }
            finally
            {
                await command.Connection.CloseAsync(); // Ensure connection is closed
            }
        }

        public Task<IEnumerable<UserToken>?> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<UserToken?> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<int> GetByAuthToken(string authToken)
        {
            await using var command = await ConnectionController.GetCommandConnection();

            if (command == null || command.Connection == null)
            {
                return -2;
            }

            command.CommandText = "SELECT \"userID\" FROM \"UserToken\" WHERE \"authToken\" = @authToken";
            ConnectionController.AddParameterWithValue(command, "authToken", DbType.String, authToken);
            try { 
                await using var reader = await command.ExecuteReaderAsync();

                var userID = -1;

                if (await reader.ReadAsync())
                {
                    userID = reader.GetInt32("userID");
                }
                return userID;
            }
            finally
            {
                await command.Connection.CloseAsync(); // Ensure connection is closed
            }

            
        }

        public Task<bool> Update(UserToken entity)
        {
            throw new NotImplementedException();
        }

    }
}
