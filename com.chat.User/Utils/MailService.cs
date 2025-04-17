using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;


namespace com.chat.User.Utils;
public class EmailService
{
        public async Task SendEmail(string toMail, string subject, string body)
        {
                try
                {
                        var factory = new ConnectionFactory { HostName = "localhost", Port = 5672 };
                        using var connection = await factory.CreateConnectionAsync();
                        using var channel = await connection.CreateChannelAsync();

                        await channel.QueueDeclareAsync(queue: "sendMail", durable: true, exclusive: false, autoDelete: false,
                            arguments: null);

                        var messageBody = new
                        {
                                toMail = toMail,
                                subject = subject,
                                messageBody = body
                        };
                        var jsonString = JsonSerializer.Serialize(messageBody); // or JsonConvert.SerializeObject if using Newtonsoft.Json

                        // Convert to byte array
                        var requestBody = Encoding.UTF8.GetBytes(jsonString);

                        await channel.BasicPublishAsync(exchange: string.Empty, routingKey: "sendMail", body: requestBody);
                        Console.WriteLine($" [x] Sent {messageBody}");

                }
                catch (Exception ex)
                {
                        Console.WriteLine(ex.Message);
                }
        }
}
