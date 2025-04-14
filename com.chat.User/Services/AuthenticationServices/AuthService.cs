using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using com.chat.User.Data;
using com.chat.User.Entity;
using com.chat.User.Utils;
using System.Data;



namespace com.chat.User.Services;


public class AuthService : IAuthService
{
        private readonly UserDbContext _context;
        private readonly PasswordHasher _passwordHasher;
        private readonly EmailService _emailService;
        public AuthService(UserDbContext context, PasswordHasher passwordHasher, EmailService emailService)
        {
                _context = context;
                _passwordHasher = passwordHasher;
                _emailService = emailService;
        }

        public async Task RegisterUser(string username, string password, string email)
        {
                try
                {
                        var conn = _context.Database.GetDbConnection();

                        if (conn.State != ConnectionState.Open)
                                await conn.OpenAsync();

                        var existingUser = await _context.UserMaster
                            .FirstOrDefaultAsync(u => u.Email == email && u.IsUserVerified == true);
                        if (existingUser != null) throw new Exception("User with this email already exists.");



                        var user = await _context.UserMaster.FirstOrDefaultAsync(u => u.Email == email);
                        string verificationToken = $"{Guid.NewGuid()}_{DateTime.UtcNow:yyyyMMddHHmmss}";

                        Console.WriteLine(verificationToken);
                        if (user != null)
                        {
                                user.Username = username;
                                user.PasswordHash = _passwordHasher.CreatePasswordHash(password);
                                user.UserVerificationToken = verificationToken;
                                user.UpdatedAt = DateTime.UtcNow;
                        }
                        else
                        {
                                var newUser = new UserInfo
                                {
                                        Email = email,
                                        Username = username,
                                        UserVerificationToken = verificationToken,
                                        Role = "User",
                                        PasswordHash = _passwordHasher.CreatePasswordHash(password),
                                        CreatedAt = DateTime.UtcNow,
                                        IsUserVerified = false
                                }
                        ;
                                _context.UserMaster.Add(newUser);
                        }
                        string link = "#";
                        await _context.SaveChangesAsync();
                        var toEmail = email;
                        var subject = "Test Email";
                        string body = $@"
                                        <!DOCTYPE html>
                                        <html>
                                        <body style=""font-family: Arial, sans-serif; color: #333; line-height: 1.6;"">
                                        <h2>Hello <strong>{username}</strong>,</h2>
                                        <p>Thank you for registering with our <strong>Chat Application</strong>. To complete your registration and verify your email address, please click the button below:</p>
                                        <div style=""text-align: center; margin: 30px 0;"">
                                        <a href=""{link}"" style=""display: inline-block; padding: 12px 24px; font-size: 16px; color: #fff; background-color: #007BFF; text-decoration: none; border-radius: 8px;"">
                                        Verify Email
                                        </a>
                                        </div>
                                        <p>If you did not sign up for this account, you can safely ignore this email.</p>
                                        <p>Best regards,<br/><strong>The Chat App Team</strong></p>
                                        </body>
                                        </html>";
                        await _emailService.SendEmailAsync(toEmail, subject, body);
                }
                catch (Exception ex)
                {

                        throw new Exception(ex.Message);
                }


        }


}