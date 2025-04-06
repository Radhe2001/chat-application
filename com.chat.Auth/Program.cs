using Microsoft.AspNetCore.Cors;


var builder = WebApplication.CreateBuilder(args);

var allowedOrigins = new[] {
    "http://localhost:5090",
    "http://localhost:5214",
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

builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();


app.Run();

