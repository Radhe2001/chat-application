namespace com.chat.Email.Services;

public interface IEmailService
{
    Task SendEmail(string toEmail, string subject, string body);
}