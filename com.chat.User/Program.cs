using com.chat.User.Data;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Microsoft.AspNetCore.Cors;
using com.chat.User.Utils;
using com.chat.User.Services;



var builder = WebApplication.CreateBuilder(args);

var allowedOrigins = new[] {
    "http://localhost:5216",
};


builder.Services.AddSingleton<EmailService>();
builder.Services.AddSingleton<PasswordHasher>();
builder.Services.AddScoped<IAuthService, AuthService>();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", builder =>
    {
        builder.WithOrigins(allowedOrigins)
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});
builder.Services.AddControllers();
builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
                    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection")))
                    );

var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();


app.Run();


