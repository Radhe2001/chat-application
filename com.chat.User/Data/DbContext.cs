using Microsoft.EntityFrameworkCore;
using com.chat.User.Entity;


namespace com.chat.User.Data;
public class UserDbContext : DbContext
{
    public DbSet<UserInfo> UserMaster { get; set; }

    public UserDbContext(DbContextOptions<UserDbContext> options)
        : base(options) { }
}