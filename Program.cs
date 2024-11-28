using UserService_test_task.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseWebSockets();
app.UseMiddleware<WebSocketController>();

app.UseAuthorization();

app.UseRouting();

app.MapControllers();

app.Run();