using Models;
using Database;
using Npgsql;

namespace User_repository
{
    internal class User_repository
    {
        public List<User> ListarUsersDB()
        {
            var list = new List<User>();

            using var connection = Databaseconnection.GetConnection();
            connection.Open();

            var query = "SELECT Id, Nome FROM users";
            using var command = new NpgsqlCommand(query, connection);
            
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var user = new User
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1)
                };

                list.Add(user);
            }

            return list;
        }
    }
}