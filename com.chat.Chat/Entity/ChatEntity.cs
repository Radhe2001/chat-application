namespace com.chat.Chat.Entity;
public class Chats
{
        public int Id { get; set; }

        public string Message { get; set; } = null!;

        public int SentBy { get; set; }

        public DateTime SentOn { get; set; }

        public bool IsGroupChat { get; set; }

        public int SentTo { get; set; }

        public bool IsReceived { get; set; } = false;

        public bool IsActive { get; set; } = true;

        public User SentByUser { get; set; } = null!;

        public User SentToUser { get; set; } = null!;
}
