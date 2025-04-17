using com.chat.Email.Services;
using Microsoft.AspNetCore.Cors;
using com.chat.Email.Events;

var builder = WebApplication.CreateBuilder(args);

var allowedOrigins = new[] {
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
builder.Services.AddHostedService<RabbitMqConsumerService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();


app.Run();


