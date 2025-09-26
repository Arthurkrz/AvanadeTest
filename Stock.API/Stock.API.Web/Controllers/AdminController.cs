using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stock.API.Core.Common;
using Stock.API.Core.Contracts.Service;
using Stock.API.Web.DTOs;

namespace Stock.API.Web.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly IValidator<AdminDTO> _adminDTOValidator;

        public AdminController(IAdminService adminService, IValidator<AdminDTO> adminDTOValidator)
        {
            _adminService = adminService;
            _adminDTOValidator = adminDTOValidator;
        }

        [HttpPost("register")]
        public IActionResult Register(AdminDTO adminDTO)
        {
            _adminDTOValidator.ValidateAndThrow(adminDTO);

            var request = new RegisterRequest(adminDTO.Username!, adminDTO.Name!,
                                              adminDTO.CPF!, adminDTO.Password!);

            var admin = _adminService.Register(request);

            return Ok(new { admin.ID, admin.Username });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] string username, [FromBody] string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)) 
                return BadRequest(ErrorMessages.EMPTYCREDENTIALS);

            var isAuthenticated = _adminService.Login(username, password);

            if (!isAuthenticated) return BadRequest(ErrorMessages.INVALIDCREDENTIALS);

            return Ok(username);
        }
    }
}
