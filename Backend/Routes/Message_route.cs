using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Backend.Services;

namespace Backend.Routes
{
    public static class ConversationMessagesRoute
    {
        public static IEndpointRouteBuilder MapConversationMessagesRoute(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/conversations/{id:int}/messages");

            group.MapGet("", GetMessages);
            group.MapPost("", PostMessage);

            return app;
        }

        private static async Task<IResult> GetMessages(
            int id,
            int? limit,
            HttpContext http,
            CancellationToken ct)
        {
            try
            {
                if (!TryGetUserId(http, out var userId))
                    return Results.Unauthorized();

                var messages = await MessageService.ListConversationMessages(id, userId);

                return Results.Ok(new { ok = true, messages });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"🔥 GET /conversations/{id}/messages error: {ex}");
                return Results.Problem(detail: ex.Message, statusCode: 500);
            }
        }


        private static async Task<IResult> PostMessage(
            int id,
            CreateMessageRequest request,
            HttpContext http,
            CancellationToken ct)
        {
            try
            {
                if (!TryGetUserId(http, out var userId))
                    return Results.Unauthorized();

                var message = await MessageService.CreateConversationMessage(id, userId, request?.Content ?? "");

                return Results.Ok(new { ok = true, message });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"POST /conversations/{id}/messages error: {ex}");
                return Results.Problem(detail: ex.Message, statusCode: 500);
            }
        }


        private static bool TryGetUserId(HttpContext http, out int userId)
        {
            userId = 0;

            var claimValue =
                http.User?.FindFirstValue(ClaimTypes.NameIdentifier) ??
                http.User?.FindFirstValue("sub");

            if (int.TryParse(claimValue, out userId) && userId > 0)
                return true;

            var item = http.Items.TryGetValue("UserId", out var v) ? v : null;
            if (item != null && int.TryParse(item.ToString(), out userId) && userId > 0)
                return true;

            return false;
        }
    }

    public sealed record CreateMessageRequest(string? Content);
}
