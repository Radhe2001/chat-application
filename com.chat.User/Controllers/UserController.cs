using com.chat.User.Models;
using Microsoft.AspNetCore.Mvc;
using com.chat.User.Services;


namespace com.chat.User.Controllers
{
        [ApiController]
        [Route("api/[controller]")]
        public class UserController : ControllerBase
        {
                private readonly IAuthService _authService;

                public UserController(IAuthService authService)
                {
                        _authService = authService;
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
        }
}
