using Npgsql;
using System;
using System.Collections.Generic;
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

    }
}
