using Backend.Routes;
using UserRoute;
using Backend.Hubs;
using Backend.Realtime;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSignalR(); 
builder.Services.AddSingleton<ChatNotifier>(); 
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy
                .WithOrigins("http://localhost:5173")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

var app = builder.Build();
app.UseCors("AllowFrontend");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapLoginRoute();
app.MapConversationRoute();
app.MapConversationMessagesRoute();
app.MapUserRoute();

app.MapHub<ChatHub>("/chatHub");

app.Run();