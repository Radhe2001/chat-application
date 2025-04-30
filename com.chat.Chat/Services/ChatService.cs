using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Text;
using System.Text.Json;
using System.Net.Http;
using com.chat.Chat.Entity;
using com.chat.Chat.Models;
using com.chat.Chat.Data;

namespace com.chat.Chat.Services;


public class ChatService : IChatService
{

    private readonly ChatDbContext _context;
    public ChatService(ChatDbContext context)
    {
        _context = context;
    }

    async public Task SetSocketId(int userId, string socketId)
    {
        User? user = await _context.UserMaster.FirstOrDefaultAsync(u => u.UserId == userId && u.SocketId != socketId);
        if (user == null) return;
        user.SocketId = socketId;
        await _context.SaveChangesAsync();
    }

    async public Task<User> GetContactList(int userId)
    {

        User? user = await _context.UserMaster.FirstOrDefaultAsync(u => u.UserId == userId);
        return user;
    }
}