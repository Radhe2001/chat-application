namespace com.chat.Chat.Entity;

public class User
{
    public int? UserId { get; set; }
    public string? Role { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public byte[]? PasswordHash { get; set; }
    public string? UserBio { get; set; }
    public string? ProfilePicture { get; set; }
    public bool IsUserVerified { get; set; }
    public string? UserVerificationToken { get; set; }
    public bool IsActive { get; set; }
    public bool HasNotification { get; set; }
    public string? SocketId { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public ICollection<Chats> SentMessages { get; set; } = new List<Chats>();
    public ICollection<Chats> ReceivedMessages { get; set; } = new List<Chats>();
}

