using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Microsoft.AspNetCore.Cors;
using com.chat.Base.Middleware;


var builder = WebApplication.CreateBuilder(args);

#region builder configuration

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

#endregion

#region service registration

var allowedOrigins = new[] {
    "http://localhost:3000",
};

builder.Services.AddCors(options =>
{
        options.AddPolicy("AllowSpecificOrigins", builder =>
        {
                builder.WithOrigins(allowedOrigins)
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});
builder.Services.AddOcelot();
builder.Services.AddHttpClient(); // 👈 Required

#endregion

var app = builder.Build();

#region middleware addition

app.UseHttpsRedirection();
app.UseRouting();
app.UseMiddleware<AuthMiddleware>();
await app.UseOcelot();
app.UseHttpsRedirection();

#endregion

app.Run();

