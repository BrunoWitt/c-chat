using Backend.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Backend.Realtime;

public class ChatNotifier
{
    private readonly IHubContext<ChatHub> _hub;

    public ChatNotifier(IHubContext<ChatHub> hub)
    {
        _hub = hub;
    }

    public async Task NotifyNewMessage(int conversationId, object message)
    {
        await _hub
            .Clients
            .Group(conversationId.ToString())
            .SendAsync("ReceiveMessage", message);
    }
}