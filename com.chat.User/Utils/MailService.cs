using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace com.chat.User.Utils;
public class EmailService
{
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
                _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
                var smtpServer = _configuration["EmailSettings:SmtpServer"];
                var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
                var smtpUser = _configuration["EmailSettings:SmtpUser"];
                var smtpPassword = _configuration["EmailSettings:SmtpPassword"];
                var fromEmail = _configuration["EmailSettings:FromEmail"];

                var mailMessage = new MailMessage
                {
                        From = new MailAddress(fromEmail),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true,
                };

                mailMessage.To.Add(toEmail);

                using (var smtpClient = new SmtpClient(smtpServer, smtpPort))
                {
                        smtpClient.Credentials = new NetworkCredential(smtpUser, smtpPassword);
                        smtpClient.EnableSsl = true;
                        await smtpClient.SendMailAsync(mailMessage);
                }
        }
}
