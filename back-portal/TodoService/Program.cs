using TodoService.Infraestructure.Clients;
using TodoService.Infraestructure.Repositories;
using TodoService.Middleware;
using TodoService.Models.Interfaces;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Services.AddSingleton<ITodoService, TodoService.Services.TodoService>();
builder.Services.AddSingleton<ITodoRepository, TodoRepository>();
builder.Services.AddSingleton<IAuthClient, AuthClient>();
builder.Services.AddSingleton<AuthMiddleware>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddHttpClient("AuthService", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Auth:BaseAddress"]
        ?? throw new Exception("Auth:BaseAddress not found"));
});

var app = builder.Build();
app.UseCors("AllowAngular");

app.UseHttpsRedirection();

app.UseMiddleware<AuthMiddleware>();

app.MapControllers();

app.Run();


namespace TodoService
{
    public partial class Program { }
}