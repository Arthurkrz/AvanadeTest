using Microsoft.AspNetCore.Mvc;
using Stock.API.Web.DTOs;
using Stock.API.Web.Utilities;

namespace Stock.API.Web.Controllers
{
    public class AuthController : ControllerBase
    {
        private readonly JwtTokenService _jwt;

        public AuthController(JwtTokenService jwt)
        {
            _jwt = jwt;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // This is just a demo. In a real application, you would validate the user credentials from a database.
            if (request.Username == "admin" && request.Password == "password")
            {
                var token = _jwt.GenerateToken(request.Username, "Admin", "Admins");
                return Ok(new { Token = token });
            }

            return Unauthorized("Invalid credentials.");
        }
    }
}
