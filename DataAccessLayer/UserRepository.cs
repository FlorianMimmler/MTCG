using System.Data;
using MTCG.BusinessLayer.Model.User;

namespace MTCG.DataAccessLayer
{
    internal class UserRepository : IRepository<User>
    {

        private static UserRepository? _instance;

        public static UserRepository Instance => _instance ??= new UserRepository();


        private UserRepository()
        {
        }

        public async Task<int> Add(User entity)
        {
            await using var conn = ConnectionController.CreateConnection();
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

        public Task<int> Delete(User entity)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<User>?> GetAll()
        {
            await using var conn = ConnectionController.CreateConnection();
            await conn.OpenAsync();
            await using var command = conn.CreateCommand();

            command.CommandText = """
                                  SELECT u.id, username, password, salt, admin, coins, stats.id as statsid, stats.eloscore, stats.wins, stats.losses
                                  FROM "User" as u
                                  LEFT JOIN "Stats" as stats on u."statsID" = stats."id"
                                  """;

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

            return null;
        }

        public User GetById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<User?> GetByUsername(string username)
        {
            await using var conn = ConnectionController.CreateConnection();
            await conn.OpenAsync();
            await using var command = conn.CreateCommand();

            command.CommandText = "SELECT id, username, password, salt FROM \"User\" WHERE username = @username";
            AddParameterWithValue(command, "username", DbType.String, username);

            await using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                var user = new User
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    Credentials = new Credentials() {
                        Username = reader.GetString("username"),
                    }
                };

                user.Credentials.SetSalt(reader.GetString("salt"));
                user.Credentials.SetPassword(reader.GetString("password"));

                return user;
            }

            return null;

        }

        public async Task<User?> GetUserDataByUsername(string username)
        {
            await using var conn = ConnectionController.CreateConnection();
            await conn.OpenAsync();
            await using var command = conn.CreateCommand();

            command.CommandText = """
                                  SELECT u.id, username, password, salt, admin, coins, stats.id as statsid, stats.eloscore, stats.wins, stats.losses
                                  FROM "User" as u
                                  LEFT JOIN "Stats" as stats on u."statsID" = stats."id"
                                  WHERE u."username" = @username
                                  """;
            AddParameterWithValue(command, "username", DbType.String, username);

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

            return null;

        }

        public async Task<User?> GetByAuthToken(string authToken)
        {
            await using var conn = ConnectionController.CreateConnection();
            await conn.OpenAsync();
            await using var command = conn.CreateCommand();

            command.CommandText = """
                                   SELECT u.id, username, password, salt, admin, coins, stats.id as statsid, stats.eloscore, stats.wins, stats.losses
                                   FROM "User" as u
                                   LEFT JOIN "UserToken" as uT on u.id = uT."userID"
                                   LEFT JOIN "Stats" as stats on u."statsID" = stats."id"
                                   WHERE uT."authToken" = @authToken
                                   """;
            AddParameterWithValue(command, "authToken", DbType.String, authToken);

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

            return null;
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
