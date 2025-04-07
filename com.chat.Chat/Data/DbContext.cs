using Microsoft.EntityFrameworkCore;
using com.chat.Chat.Entity;


namespace com.chat.Chat.Data;
public class ChatDbContext : DbContext
{
        public DbSet<ChatEntity> UserMaster { get; set; }

        public ChatDbContext(DbContextOptions<ChatDbContext> options)
            : base(options) { }
}