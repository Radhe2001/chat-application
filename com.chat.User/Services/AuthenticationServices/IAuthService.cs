using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace com.chat.User.Services;
public interface IAuthService
{
        Task RegisterUser(string username, string password, string email);
}