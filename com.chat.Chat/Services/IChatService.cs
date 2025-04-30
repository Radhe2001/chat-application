using com.chat.Chat.Models;
using com.chat.Chat.Entity;

namespace com.chat.Chat.Services;
public interface IChatService
{
    Task<User> GetContactList(int userId);
    Task SetSocketId(int userId, string socketId);
}