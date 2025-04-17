using com.chat.User.Models;
using Microsoft.AspNetCore.Mvc;
using com.chat.User.Services;
using RabbitMQ.Client;
using System.Text;
using com.chat.User.Utils;

namespace com.chat.User.Controllers
{
        [ApiController]
        [Route("api/[controller]")]
        public class UserController : ControllerBase
        {
                private readonly IAuthService _authService;
                private readonly EmailService _emailService;

                public UserController(IAuthService authService, EmailService emailService)
                {
                        _authService = authService;
                        _emailService = emailService;
                }

                [HttpGet("GetRoleAndId")]
                public ActionResult GetUsers()
                {
                        // var userId = HttpContext.Request.Headers["X-User-Id"].ToString();
                        // var role = HttpContext.Request.Headers["X-User-Role"].ToString();
                        // return Ok(new { userId = userId, role = role });
                        return Ok();
                }

                [HttpPost("register")]
                public async Task<ActionResult> Register([FromBody] UserAuthInput input)
                {
                        try
                        {
                                await _authService.RegisterUser(input.username, input.password, input.email);

                                return Ok("Account Registered Successfully");
                        }
                        catch (Exception e)
                        {
                                if (e.Message.Contains("User with this email already exists.")) return Conflict(e.Message);
                                return StatusCode(500, "Internal server error occurred." + e.Message);
                        }
                }

                [HttpPost("accountVerification")]
                public async Task<ActionResult> AccountVerification([FromBody] UserAuthInput input)
                {
                        try
                        {
                                if (input.taskType == "tokenValidation")
                                {
                                        await _authService.ValidateAccountVerficationToken(input.verificationToken);
                                        return Ok("Token Exists");
                                }
                                else if (input.taskType == "accountDeauth")
                                {
                                        await _authService.AccountDeauthentication(input.verificationToken);
                                        return Ok("Account Verified Successfully");
                                }
                                else
                                {
                                        await _authService.AccountVerification(input.verificationToken);
                                        return Ok("Account Verified Successfully");
                                }
                        }
                        catch (Exception e)
                        {
                                if (e.Message.Contains("Account does not exists.")) return NotFound(e.Message);
                                return StatusCode(500, "Internal server error occurred." + e.Message);
                        }
                }

                [HttpPost("login")]
                public async Task<IActionResult> UserLogin([FromBody] UserAuthInput input)
                {
                        try
                        {
                                var token = await _authService.UserLogin(input.email, input.password);
                                if (token == "User does not exists") return NotFound(token);
                                if (token == "Password is not valid") return BadRequest(token);
                                return Ok(token);
                        }
                        catch (Exception e)
                        {
                                return StatusCode(500, "Internal server error occurred." + e.Message);
                        }
                }


                [HttpPost("forgotPassword")]
                public async Task<IActionResult> ForgotPassword([FromBody] UserAuthInput input)
                {
                        try
                        {
                                var res = await _authService.ForgotPassword(input.email);
                                if (res == "User does not exists.") return NotFound(res);
                                else return Ok("Reset password mail has been sent successfully");
                        }
                        catch (Exception e)
                        {
                                return StatusCode(500, "Internal server error occurred." + e.Message);
                        }
                }

                [HttpPost("resetPassword")]
                public async Task<IActionResult> ResetPassword([FromBody] UserAuthInput input)
                {
                        try
                        {
                                var res = await _authService.ResetPassword(input.email, input.password);
                                if (res == "User does not exists.") return NotFound(res);
                                else return Ok("Password has been reset successfully");
                        }
                        catch (Exception e)
                        {
                                if (e.Message.Contains("Account does not exists.")) return NotFound(e.Message);
                                return StatusCode(500, "Internal server error occurred." + e.Message);
                        }
                }

        }
}
