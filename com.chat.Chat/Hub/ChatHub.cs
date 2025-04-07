using Microsoft.AspNetCore.SignalR;

namespace com.chat.Chat.Hubs;

public class ChatHub : Hub
{
        public override async Task OnConnectedAsync()
        {
                var httpContext = Context.GetHttpContext();
                Console.WriteLine(">>> Incoming connection...");
                if (httpContext is not null)
                {
                        Console.WriteLine("Headers:");
                        foreach (var header in httpContext.Request.Headers)
                        {
                                Console.WriteLine($"{header.Key}: {header.Value}");
                        }
                }

                await base.OnConnectedAsync();
        }



        public async Task SendMessage(string user, string message)
        {
                Console.WriteLine($"Connected:  | Role: ");

                await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public Task CreateConnection(string token)
        {
                Console.WriteLine("Received token: " + token);
                return Task.CompletedTask;
        }
}
