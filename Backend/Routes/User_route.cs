using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Services; // <- aqui está seu UserService

namespace UserRoute;

public static class UserRoute
{
    public static IEndpointRouteBuilder MapUserRoute(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/user");

        group.MapGet("/", List);

        return app;
    }

    private static Task<IResult> List()
    {
        var users = UserService.ListarUsers();
        return Task.FromResult(Results.Ok(users));
    }
}
