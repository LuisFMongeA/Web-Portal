using UserService.Infraestructure.Clients;
using UserService.Infraestructure.Repositories;
using UserService.Models.Interfaces;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddSingleton<IUserService, UserService.Services.UserService>();
builder.Services.AddSingleton<IAuthClient, AuthClient>();
builder.Services.AddSingleton<IUserRepository, UserRepository>();

builder.Services.AddHttpClient("AuthService", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Auth:BaseAddress"]
        ?? throw new Exception("Auth:BaseAddress not found"));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors("AllowAngular");

app.UseHttpsRedirection();



app.MapControllers();

app.Run();

namespace UserService
{
    public partial class Program { }
}