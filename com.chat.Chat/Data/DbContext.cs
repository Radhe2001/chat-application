using Microsoft.EntityFrameworkCore;
using com.chat.Chat.Entity;


namespace com.chat.Chat.Data;
public class ChatDbContext : DbContext
{
    public DbSet<Chats> ChatMessage { get; set; }
    public DbSet<User> UserMaster { get; set; }

    public ChatDbContext(DbContextOptions<ChatDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Chats>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Message).IsRequired();

            entity.Property(e => e.SentOn).IsRequired();

            entity.Property(e => e.IsGroupChat).IsRequired();

            entity.Property(e => e.IsReceived).HasDefaultValue(false);

            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(e => e.SentByUser)
                  .WithMany(u => u.SentMessages)
                  .HasForeignKey(e => e.SentBy)
                  .HasConstraintName("FK_ChatMessages_SentBy")
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.SentToUser)
                  .WithMany(u => u.ReceivedMessages)
                  .HasForeignKey(e => e.SentTo)
                  .HasConstraintName("FK_ChatMessages_SentTo")
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId);
            entity.Property(e => e.Role);
            entity.Property(e => e.Username);
            entity.Property(e => e.Email);
            entity.Property(e => e.PasswordHash);
            entity.Property(e => e.UserBio);
            entity.Property(e => e.ProfilePicture);
            entity.Property(e => e.IsUserVerified);
            entity.Property(e => e.UserVerificationToken);
            entity.Property(e => e.IsActive);
            entity.Property(e => e.HasNotification);
            entity.Property(e => e.SocketId);
            entity.Property(e => e.CreatedAt);
            entity.Property(e => e.UpdatedAt);
        });

    }

}