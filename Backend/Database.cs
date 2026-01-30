using Npgsql;

namespace Database
{
    internal class Databaseconnection
    {
        public static NpgsqlConnection GetConnection()
        {
            string connection = "Host=localhost;Port=5432;Username=postgres;Password=1234;Database=teamChat;";


            return new NpgsqlConnection(connection);
        }
    }
}