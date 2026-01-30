using Models;

namespace Backend.Services;

public static class LoginService
{
    internal static Task<User?> LoginAsync(string username)
    {
        var repo = new Login_repository.Login_repository();
        return Task.FromResult(repo.getUserDB(username));
    }
}