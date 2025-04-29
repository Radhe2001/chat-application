using com.chat.Base.Middleware;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Microsoft.AspNetCore.Cors;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("ocelot.json");

var allowedOrigins = new[] { "http://localhost:3000" };

builder.Services.AddCors(options =>
{
        options.AddPolicy("AllowSpecificOrigins", policy =>
        {
                policy.WithOrigins(allowedOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

builder.Services.AddHttpClient();
builder.Services.AddOcelot();

var app = builder.Build();

app.UseRouting();
app.UseCors("AllowSpecificOrigins");
app.UseWebSockets();
app.UseMiddleware<AuthMiddleware>();

await app.UseOcelot();


app.Run();