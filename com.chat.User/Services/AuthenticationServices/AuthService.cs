using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Text;
using System.Text.Json;
using System.Net.Http;
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
        private readonly HttpClient _httpClient;

        public AuthService(UserDbContext context, PasswordHasher passwordHasher, EmailService emailService, IHttpClientFactory httpClientFactory)
        {
                _context = context;
                _passwordHasher = passwordHasher;
                _emailService = emailService;
                _httpClient = httpClientFactory.CreateClient();
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
                                };
                                _context.UserMaster.Add(newUser);
                        }
                        string link = "http://localhost:3000/auth/register/" + verificationToken;
                        await _context.SaveChangesAsync();
                        var toEmail = email;
                        var subject = "Account verification mail";
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
                        _emailService.SendEmail(toEmail, subject, body);
                }
                catch (Exception ex)
                {
                        throw new Exception(ex.Message);
                }
        }


        public async Task ValidateAccountVerficationToken(string verificationToken)
        {
                try
                {
                        var user = await _context.UserMaster.FirstOrDefaultAsync(u => u.UserVerificationToken == verificationToken);
                        if (user == null) throw new Exception("Account does not exists.");
                }
                catch (Exception ex)
                {
                        throw new Exception(ex.Message);
                }
        }
        public async Task AccountVerification(string verificationToken)
        {
                try
                {
                        UserInfo? user = await _context.UserMaster.FirstOrDefaultAsync(u => u.UserVerificationToken == verificationToken);
                        if (user == null) throw new Exception("Account does not exists.");
                        user.IsUserVerified = true;
                        await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                        throw new Exception(ex.Message);
                }

        }

        public async Task AccountDeauthentication(string verificationToken)
        {
                try
                {
                        var user = await _context.UserMaster.FirstOrDefaultAsync(u => u.UserVerificationToken == verificationToken);
                        if (user == null) throw new Exception("Account does not exists.");
                        _context.UserMaster.Remove(user);
                        await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                        throw new Exception(ex.Message);
                }

        }

        public async Task<string> UserLogin(string email, string password)
        {
                try
                {
                        var user = await _context.UserMaster.FirstOrDefaultAsync(u => u.Email == email && u.IsUserVerified == true);
                        if (user == null) return "User does not exists";
                        bool isPasswordCorrect = _passwordHasher.VerifyPassword(password, user.PasswordHash);
                        if (!isPasswordCorrect) return "Password is not valid";

                        var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5090/api/Auth/create-token");

                        var payload = new
                        {
                                UserId = user.UserId,
                                Role = user.Role,
                                Email = user.Email
                        };

                        string jsonPayload = JsonSerializer.Serialize(payload);
                        request.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                        var response = await _httpClient.SendAsync(request);

                        if (!response.IsSuccessStatusCode)
                        {
                                throw new Exception("Token service returned an error: " + response.StatusCode);
                        }

                        var token = await response.Content.ReadAsStringAsync();
                        return token;
                }
                catch (Exception ex)
                {
                        throw new Exception(ex.Message);
                }
        }

        public async Task<string> ForgotPassword(string email)
        {
                try
                {
                        var user = await _context.UserMaster
                            .FirstOrDefaultAsync(u => u.Email == email && u.IsUserVerified == true);
                        if (user == null) return "User does not exists.";


                        string link = "http://localhost:3000/auth/forgotPassword/" + user.UserVerificationToken;
                        var toEmail = email;
                        var subject = "Reset Password";
                        string body = $@"
                                <!DOCTYPE html>
                                <html>
                                <body style=""font-family: Arial, sans-serif; color: #333; line-height: 1.6;"">
                                <h2>Hello <strong>{user.Username}</strong>,</h2>
                                <p>We received a request to reset your password for your <strong>Chat Application</strong> account. To reset your password, please click the button below:</p>
                                <div style=""text-align: center; margin: 30px 0;"">
                                <a href=""{link}"" style=""display: inline-block; padding: 12px 24px; font-size: 16px; color: #fff; background-color: #007BFF; text-decoration: none; border-radius: 8px;"">
                                Reset Password
                                </a>
                                </div>
                                <p>If you did not request a password reset, please ignore this email. Your password will not be changed.</p>
                                <p>If you have any issues, feel free to contact our support team.</p>
                                <p>Best regards,<br/><strong>The Chat App Team</strong></p>
                                </body>
                                </html>";

                        await _emailService.SendEmail(toEmail, subject, body);
                        return "success";
                }
                catch (Exception ex)
                {
                        throw new Exception(ex.Message);
                }
        }


        public async Task<string> ResetPassword(string verificationToken, string password)
        {
                try
                {
                        var user = await _context.UserMaster.FirstOrDefaultAsync(u => u.UserVerificationToken == verificationToken && u.IsUserVerified == true);
                        if (user == null) throw new Exception("Account does not exists.");
                        user.PasswordHash = _passwordHasher.CreatePasswordHash(password);
                        await _context.SaveChangesAsync();
                        return "success";
                }
                catch (Exception ex)
                {
                        throw new Exception(ex.Message);
                }

        }

}