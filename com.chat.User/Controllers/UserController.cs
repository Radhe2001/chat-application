using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using com.chat.User.Data;
using com.chat.User.Entity;


namespace com.chat.User.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
        private readonly UserDbContext _context;

        public UserController(UserDbContext context)
        {
                _context = context;
        }

        [HttpGet("GetRoleAndId")]
        public async Task<ActionResult> GetUsers()
        {
                var userId = HttpContext.Request.Headers["X-User-Id"].ToString();
                var role = HttpContext.Request.Headers["X-User-Role"].ToString();
                return Ok(new { userId = userId, role = role });
        }


        [HttpGet("login")]
        public async Task<ActionResult> Login()
        {

                return Ok("Logged in without token");
        }


}
