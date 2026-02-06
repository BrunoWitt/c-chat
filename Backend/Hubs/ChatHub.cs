using Microsoft.AspNetCore.SignalR;

namespace Backend.Hubs;

public class ChatHub : Hub
{
    public async Task JoinConversation(string conversationId)
    {
        await Groups.AddToGroupAsync(
            Context.ConnectionId,
            conversationId
        );
    }

    public async Task SendMessage(string conversationId, object message)
    {
        await Clients
            .Group(conversationId)
            .SendAsync("ReceiveMessage", message);
    }
}