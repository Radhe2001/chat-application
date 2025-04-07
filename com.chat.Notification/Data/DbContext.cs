using Microsoft.EntityFrameworkCore;
using com.chat.Notification.Entity;


namespace com.chat.Notification.Data;
public class NotificationDbContext : DbContext
{
        public DbSet<NotificationEntity> Notifications { get; set; }

        public NotificationDbContext(DbContextOptions<NotificationDbContext> options)
            : base(options) { }
}