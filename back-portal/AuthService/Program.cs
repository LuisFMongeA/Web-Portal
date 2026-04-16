using AuthService.Models.Interfaces;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IAuthService, AuthService.Services.AuthService>();

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

namespace AuthService {
    public partial class Program { }
}
