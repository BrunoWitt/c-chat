using Backend.Services;
using Microsoft.AspNetCore.Http;
using Services;

namespace Backend.Routes;

public static class ConversationRoute
{
    public sealed record CreateConversationRequest(int OtherUserId);
    public sealed record ConversationDto(int Id, DateTime CreatedAt);

    private static bool TryGetUserId(HttpContext ctx, out int userId)
    {
        userId = 0;

        if (!ctx.Request.Headers.TryGetValue("x-user-id", out var values))
            return false;

        return int.TryParse(values.ToString(), out userId) && userId > 0;
    }


    public static IEndpointRouteBuilder MapConversationRoute(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/auth");

        group.MapGet("/conversations", async (HttpContext ctx) =>
        {
            if (!TryGetUserId(ctx, out var userId))
                return Results.Json(new { ok = false, error = "x-user-id obrigatório" }, statusCode: 401);

            try
            {
                var conversations = await ConversationService.ListConversationsAsync(userId);

                var dto = conversations.ConvertAll(c => new ConversationDto(c.id, c.createdAt));

                return Results.Json(new { ok = true, conversations = dto });
            }
            catch (Exception ex)
            {
                return Results.Json(new { ok = false, error = ex.Message }, statusCode: 500);
            }
        });


        group.MapPost("/conversations", async (HttpContext ctx, CreateConversationRequest body) =>
        {
            if (!TryGetUserId(ctx, out var userId))
                return Results.Json(new { ok = false, error = "x-user-id obrigatório" }, statusCode: 401);

            try
            {
                var conversationId = await ConversationService.GetOrCreatePVAsync(userId, body.OtherUserId);
                return Results.Json(new { ok = true, conversation = conversationId });
            }
            catch (Exception ex)
            {
                return Results.Json(new { ok = false, error = ex.Message }, statusCode: 400);
            }
        });

        return app;
    }
}
