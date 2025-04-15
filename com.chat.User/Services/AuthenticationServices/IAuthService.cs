using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace com.chat.User.Services;
public interface IAuthService
{
        Task RegisterUser(string username, string password, string email);
        Task ValidateAccountVerficationToken(string verificationToken);
        Task AccountVerification(string verificationToken);
        Task AccountDeauthentication(string verificationToken);
        Task<string> UserLogin(string email, string password);
        Task<string> ForgotPassword(string email);
        Task<string> ResetPassword(string email, string password);


}