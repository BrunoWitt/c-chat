using Backend.Routes;
using UserRoute;
using Backend.Hubs;
using Backend.Realtime;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSignalR(); 

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapLoginRoute();
app.MapConversationRoute();
app.MapConversationMessagesRoute();
app.MapUserRoute();
builder.Services.AddSingleton<ChatNotifier>(); 

app.MapHub<ChatHub>("/ws/chat"); // 👈 HUB

app.Run();