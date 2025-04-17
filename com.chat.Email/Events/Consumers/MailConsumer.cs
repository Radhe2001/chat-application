using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using com.chat.Email.Models;
using com.chat.Email.Services;

namespace com.chat.Email.Events;

public class RabbitMqConsumerService : BackgroundService
{

    private readonly IEmailService _emailService;

    public RabbitMqConsumerService(IEmailService emailService)
    {
        _emailService = emailService;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            var factory = new ConnectionFactory { HostName = "localhost", Port = 5672 };
            var connection = await factory.CreateConnectionAsync();
            var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(queue: "sendMail",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
            Console.WriteLine("MQ CONNECTION STARTED");
            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                EmailInput input = JsonSerializer.Deserialize<EmailInput>(message);
                await _emailService.SendEmail(input.toMail, input.subject, input.messageBody);

                await Task.Yield();
            };

            await channel.BasicConsumeAsync(queue: "sendMail", autoAck: true, consumer: consumer);

        }
        catch (Exception ex)
        {
            Console.WriteLine($"RabbitMQ connection failed: {ex.Message}");
        }

        await Task.Delay(0, stoppingToken);
    }


}
