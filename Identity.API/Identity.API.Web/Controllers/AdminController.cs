using FluentValidation;
using Identity.API.Core.Common;
using Identity.API.Core.Contracts.Service;
using Identity.API.Web.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Web.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AdminController : Controller
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
        var validationResult = _adminDTOValidator.Validate(adminDTO);

        if (!validationResult.IsValid) return BadRequest(ErrorMessages.INVALIDREQUEST
            .Replace("{error}", string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))));

        var request = new RegisterRequest(adminDTO.Username!, adminDTO.Name!,
                                          adminDTO.CPF!, adminDTO.Password!);

        var admin = _adminService.Register(request);

        return Ok(new { admin.ID, admin.Username });
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest loginRequest)
    {
        var username = loginRequest.Username;
        var password = loginRequest.Password;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            return BadRequest(ErrorMessages.EMPTYCREDENTIALS);

        var isAuthenticated = _adminService.Login(username, password);

        if (!isAuthenticated) return BadRequest(ErrorMessages.INVALIDCREDENTIALS);

        return Ok(username);
    }
}
