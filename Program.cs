using UserService_test_task.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseWebSockets();

app.UseAuthorization();

app.UseRouting();

app.MapControllers();

app.Run();
