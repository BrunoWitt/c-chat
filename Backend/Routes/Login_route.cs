using Backend.Services;
using Models;

namespace Backend.Routes;

public static class LoginRoute
{
    public static IEndpointRouteBuilder MapLoginRoute(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/auth");
        group.MapPost("/login", Login);
        return app;
    }

    private static async Task<IResult> Login(LoginRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Username))
            return Results.BadRequest(new { success = false, message = "Username é obrigatório." });

        Models.User? user = await LoginService.LoginAsync(request.Username);

        if (user is null)
            return Results.Unauthorized();

        return Results.Ok(new
        {
            success = true,
            user = new { user.Id, user.Name }
        });
    }
}

public sealed record LoginRequest(string Username);
