using System.Data;
using System.Data.Common;
using MTCG.Auth;
using MTCG.BusinessLayer.Model.User;
using Npgsql;

namespace MTCG.DataAccessLayer
{
    public class UserRepository : IUserRepository
    {

        private static IUserRepository? _instance;

        public static IUserRepository Instance
        {
            get => _instance ??= new UserRepository();
            set => _instance = value; // Allow override for testing
        }


        private UserRepository()
        {
        }

        public async Task<int> Add(User entity)
        {
            await using var command = await ConnectionController.GetCommandConnection();

            if (command == null || command.Connection == null)
            {
                return -2;
            }

            command.CommandText = "INSERT INTO \"User\" (username, password, salt, \"statsID\") " +
                                  "VALUES (@username, @password, @salt, @statsID) RETURNING id";
            ConnectionController.AddParameterWithValue(command, "username", DbType.String, entity.GetName());
            ConnectionController.AddParameterWithValue(command, "password", DbType.String, entity.Credentials.Password);
            ConnectionController.AddParameterWithValue(command, "salt", DbType.String, entity.Credentials.Salt);
            ConnectionController.AddParameterWithValue(command, "statsID", DbType.Int32, entity.Stats.Id);
            try { 
                return (int)(await command.ExecuteScalarAsync() ?? 0);
            }
            finally
            {
                await command.Connection.CloseAsync(); // Ensure connection is closed
            }

        }

        public Task<int> Delete(User entity)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<User>?> GetAll()
        {
            await using var command = await ConnectionController.GetCommandConnection();

            if (command == null || command.Connection == null)
            {
                return null; //TODO
            }

            command.CommandText = """
                                  SELECT u.id, username, password, salt, admin, coins, stats.id as statsid, stats.eloscore, stats.wins, stats.losses
                                  FROM "User" as u
                                  LEFT JOIN "Stats" as stats on u."statsID" = stats."id"
                                  """;
            try { 
                await using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    var allUsers = new List<User>();
                    do
                    {
                        var user = new User
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Credentials = new Credentials()
                            {
                                Username = reader.GetString("username"),
                            },
                            Coins = reader.GetInt32("coins"),
                            Stats = new Stats()
                            {
                                Elo = new Elo() { EloScore = reader.GetInt32("eloscore") },
                                Wins = reader.GetInt32("wins"),
                                Losses = reader.GetInt32("losses"),
                                Id = reader.GetInt32("statsid")
                            },
                            Admin = reader.GetBoolean("admin")
                        };

                        user.Credentials.SetSalt(reader.GetString("salt"));
                        user.Credentials.SetPassword(reader.GetString("password"));

                        allUsers.Add(user);

                    } while (await reader.ReadAsync());

                    return allUsers;
                }
            }
            finally
            {
                await command.Connection.CloseAsync(); // Ensure connection is closed
            }

            return null;
        }

        public Task<User?> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<User?> GetByUsername(string username)
        {
            
            await using var command = await ConnectionController.GetCommandConnection();

            if(command == null)
            {
                return new User(new Credentials("__connection__error__", ""));
            }

            command.CommandText = "SELECT id, username, password, salt FROM \"User\" WHERE username = @username";
            ConnectionController.AddParameterWithValue(command, "username", DbType.String, username);

            try { 
            await using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                var user = new User
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    Credentials = new Credentials()
                    {
                        Username = reader.GetString("username"),
                    }
                };

                user.Credentials.SetSalt(reader.GetString("salt"));
                user.Credentials.SetPassword(reader.GetString("password"));

                return user;
            }
            }
            finally
            {
                await command.Connection.CloseAsync(); // Ensure connection is closed
            }

            return null;


        }

        public async Task<User?> GetUserDataByUsername(string username)
        {
            await using var command = await ConnectionController.GetCommandConnection();

            if (command == null || command.Connection == null)
            {
                return new User(new Credentials("__connection__error__", ""));
            }

            command.CommandText = """
                                  SELECT u.id, username, password, salt, admin, coins, stats.id as statsid, stats.eloscore, stats.wins, stats.losses
                                  FROM "User" as u
                                  LEFT JOIN "Stats" as stats on u."statsID" = stats."id"
                                  WHERE u."username" = @username
                                  """;
            ConnectionController.AddParameterWithValue(command, "username", DbType.String, username);
            try { 
                await using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    var user = new User
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("id")),
                        Credentials = new Credentials()
                        {
                            Username = reader.GetString("username"),
                        },
                        Coins = reader.GetInt32("coins"),
                        Stats = new Stats()
                        {
                            Elo = new Elo() { EloScore = reader.GetInt32("eloscore") },
                            Wins = reader.GetInt32("wins"),
                            Losses = reader.GetInt32("losses"),
                            Id = reader.GetInt32("statsid")
                        },
                        Admin = reader.GetBoolean("admin")
                    };

                    user.Credentials.SetSalt(reader.GetString("salt"));
                    user.Credentials.SetPassword(reader.GetString("password"));

                    return user;
                }
            }
            finally
            {
                await command.Connection.CloseAsync(); // Ensure connection is closed
            }

            return null;

        }

        public async Task<User?> GetByAuthToken(string authToken)
        {
            await using var command = await ConnectionController.GetCommandConnection();

            if (command == null || command.Connection == null)
            {
                return new User(new Credentials("__connection__error__", ""));
            }

            command.CommandText = """
                                   SELECT u.id, username, password, salt, admin, coins, stats.id as statsid, stats.eloscore, stats.wins, stats.losses
                                   FROM "User" as u
                                   LEFT JOIN "UserToken" as uT on u.id = uT."userID"
                                   LEFT JOIN "Stats" as stats on u."statsID" = stats."id"
                                   WHERE uT."authToken" = @authToken
                                   """;
            ConnectionController.AddParameterWithValue(command, "authToken", DbType.String, authToken);
            try { 
                await using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    var user = new User
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("id")),
                        Credentials = new Credentials()
                        {
                            Username = reader.GetString("username"),
                        },
                        Coins = reader.GetInt32("coins"),
                        Stats = new Stats()
                        {
                            Elo = new Elo() { EloScore = reader.GetInt32("eloscore") },
                            Wins = reader.GetInt32("wins"),
                            Losses = reader.GetInt32("losses"),
                            Id = reader.GetInt32("statsid")
                        },
                        Admin = reader.GetBoolean("admin")
                    };

                    user.Credentials.SetSalt(reader.GetString("salt"));
                    user.Credentials.SetPassword(reader.GetString("password"));

                    return user;
                }
            }
            finally
            {
                await command.Connection.CloseAsync(); // Ensure connection is closed
            }

            return null;
        }

        public async Task<bool> Update(User entity)
        {
            await using var command = await ConnectionController.GetCommandConnection();

            if (command == null || command.Connection == null)
            {
                return false; //TODO
            }

            command.CommandText =
                "UPDATE \"User\" SET username = @username, password = @password WHERE \"id\" = @id";
            ConnectionController.AddParameterWithValue(command, "username", DbType.String, entity.Credentials.Username);
            ConnectionController.AddParameterWithValue(command, "password", DbType.String, entity.Credentials.Password);
            ConnectionController.AddParameterWithValue(command, "id", DbType.Int32, entity.Id);

            try
            {
                return await command.ExecuteNonQueryAsync() == 1;
            }
            catch (Exception)
            {
                // Any other exceptions
                return false;
            }
            finally
            {
                await command.Connection.CloseAsync(); // Ensure connection is closed
            }
        }

        public async Task<bool> UpdateCoins(int newCoinsCount, int userId)
        {
            await using var command = await ConnectionController.GetCommandConnection();

            if (command == null || command.Connection == null)
            {
                return false; //TODO
            }

            command.CommandText =
                "UPDATE \"User\" SET coins = @coinsCount WHERE \"id\" = @id";
            ConnectionController.AddParameterWithValue(command, "coinsCount", DbType.Int32, newCoinsCount);
            ConnectionController.AddParameterWithValue(command, "id", DbType.Int32, userId);
            int result;
            try
            {
                 result = await command.ExecuteNonQueryAsync();
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                await command.Connection.CloseAsync(); // Ensure connection is closed
            }

            return result == 1;
        }

    }
}
