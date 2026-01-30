using Models;
using Database;
using Npgsql;

namespace Login_repository
{
    internal class Login_repository
    {
        public User? getUserDB(string userName)   // <-- User?
        {
            using var connection = Databaseconnection.GetConnection();
            connection.Open();

            var query = "SELECT * FROM users WHERE name = @username;";
            using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("username", userName);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new User
                {
                    Id = reader.GetInt64(reader.GetOrdinal("id")),
                    Name = reader.GetString(reader.GetOrdinal("name")),
                };
            }

            return null;
        }
    }
}
