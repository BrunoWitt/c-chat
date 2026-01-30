using Models;

namespace Services
{
    public static class UserService
    {
        public static List<User> ListarUsers()
        {
            var repo = new global::User_repository.User_repository();

            return repo.ListarUsersDB();
        }
    }
}
