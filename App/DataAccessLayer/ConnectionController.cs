using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.DataAccessLayer
{
    internal class ConnectionController
    {

        private static ConnectionController? _instance;

        public static ConnectionController Instance => _instance ??= new ConnectionController();

        public ConnectionController() { }



        private const string ConnectionString = "Host=localhost;Port=5432;Username=admin;Password=password;Database=MTCG";

        public static NpgsqlConnection CreateConnection()
        {
            return new NpgsqlConnection(ConnectionString);
        }

        public static void AddParameterWithValue(IDbCommand command, string parameterName, DbType type, object value)
        {
            var parameter = command.CreateParameter();
            parameter.DbType = type;
            parameter.ParameterName = parameterName;
            parameter.Value = value ?? DBNull.Value;
            command.Parameters.Add(parameter);
        }

        public async static Task<NpgsqlCommand?> GetCommandConnection()
        {
            try
            {
                var conn = CreateConnection();
                await conn.OpenAsync();
                return conn.CreateCommand();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
