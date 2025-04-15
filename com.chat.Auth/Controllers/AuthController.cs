using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using com.chat.Auth.Models;

namespace com.chat.Auth.Controller;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
        private readonly IConfiguration _config;

        public AuthController(IConfiguration config)
        {
                _config = config;
        }

        [HttpPost("validate")]
        public IActionResult ValidateToken()
        {
                var authHeader = Request.Headers["Authorization"].FirstOrDefault();

                if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
                        return Unauthorized("Missing or invalid Authorization header");

                var token = authHeader.Substring("Bearer ".Length);

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"]);

                try
                {
                        tokenHandler.ValidateToken(token, new TokenValidationParameters
                        {
                                ValidateIssuer = true,
                                ValidateAudience = true,
                                ValidIssuer = _config["Jwt:Issuer"],
                                ValidAudience = _config["Jwt:Audience"],
                                IssuerSigningKey = new SymmetricSecurityKey(key),
                                ValidateLifetime = true,
                                ClockSkew = TimeSpan.Zero
                        }, out SecurityToken validatedToken);

                        var jwtToken = (JwtSecurityToken)validatedToken;

                        var userId = jwtToken.Claims.First(x => x.Type == "userId").Value;
                        var role = jwtToken.Claims.First(x => x.Type == ClaimTypes.Role).Value;

                        return Ok(new
                        {
                                userId,
                                role
                        });
                }
                catch
                {
                        return Unauthorized("Invalid token");
                }
        }

        [HttpPost("create-token")]
        public IActionResult CreateToken([FromBody] TokenModel input)
        {
                var claims = new[]
                {
                        new Claim("userId", input.UserId.ToString()),
                         new Claim("email", input.Email.ToString()),
                        new Claim(ClaimTypes.Role, input.Role)
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _config["Jwt:Issuer"],
                    audience: _config["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(1),
                    signingCredentials: creds
                );

                var jwt = new JwtSecurityTokenHandler().WriteToken(token);
                return Ok(new { token = jwt });
        }

}
