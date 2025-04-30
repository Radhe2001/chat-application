using Microsoft.AspNetCore.SignalR;
using com.chat.Chat.Services;
using com.chat.Chat.Models;
using com.chat.Chat.Entity;


namespace com.chat.Chat.Hubs;

public class ChatHub : Hub
{
        private readonly IChatService _chatService;
        public ChatHub(IChatService chatService)
        {
                _chatService = chatService;
        }

        public override async Task OnConnectedAsync()
        {
                var httpContext = Context.GetHttpContext();
                if (httpContext is not null)
                {
                        int userId = Convert.ToInt32(httpContext?.Request.Headers["X-User-Id"]);
                        await _chatService.SetSocketId(userId, Context.ConnectionId.ToString());
                        await Clients.Caller.SendAsync("UserDetails", userId);
                }
                await base.OnConnectedAsync();
        }


        public async Task GetContactList(int userId)
        {
                User contactList = await _chatService.GetContactList(userId);
                await Clients.All.SendAsync("ReceiveMessage", contactList);
        }

        public async Task SendMessage(string message)
        {
                Console.WriteLine($"Connected:  | Role: {message}");

                await Clients.All.SendAsync("ReceiveMessage", message);
        }

        public Task CreateConnection(string token)
        {
                Console.WriteLine("Received token: " + token);
                return Task.CompletedTask;
        }
}
