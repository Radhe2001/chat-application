using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace com.chat.User.Utils;
public class PasswordHasher
{
    private readonly byte[] _salt;
    private readonly string _pepper;

    public PasswordHasher(IConfiguration configuration)
    {
        _salt = Convert.FromBase64String(configuration["PasswordHashing:Salt"]);
        _pepper = configuration["PasswordHashing:Pepper"];
    }

    public byte[] CreatePasswordHash(string password)
    {
        using var hmac = new HMACSHA512(_salt);
        var combinedPassword = password + _pepper;
        return hmac.ComputeHash(Encoding.UTF8.GetBytes(combinedPassword));
    }

    public bool VerifyPassword(string password, byte[] storedHash)
    {
        using var hmac = new HMACSHA512(_salt);
        var combinedPassword = password + _pepper;
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(combinedPassword));
        return computedHash.SequenceEqual(storedHash);
    }
}
