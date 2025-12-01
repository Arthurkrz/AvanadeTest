using FluentValidation;
using Identity.API.Core.Common;
using Identity.API.Core.Contracts.Service;
using Identity.API.Web.DTOs;
using Identity.API.Web.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Web.Controllers;

[Authorize]
[ApiController]
[Route("identity")]
public class AdminController : Controller
{
    private readonly IAdminService _adminService;
    private readonly IValidator<AdminDTO> _adminDTOValidator;
    private readonly JwtTokenService _jwtService;

    public AdminController(IAdminService adminService, IValidator<AdminDTO> adminDTOValidator, JwtTokenService jwtService)
    {
        _adminService = adminService;
        _adminDTOValidator = adminDTOValidator;
        _jwtService = jwtService;
    }

    [AllowAnonymous]
    [HttpPost("admin/register")]
    public async Task<IActionResult> Register(AdminDTO adminDTO)
    {
        var validationResult = _adminDTOValidator.Validate(adminDTO);

        if (!validationResult.IsValid) return BadRequest(ErrorMessages.INVALIDREQUEST
            .Replace("{error}", string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))));

        var request = new AdminRegisterRequest(adminDTO.Username!, adminDTO.Name!,
                                               adminDTO.CPF!, adminDTO.Password!);

        var admin = await _adminService.RegisterAsync(request);

        return Ok(new { admin.Username });
    }

    [AllowAnonymous]
    [HttpPost("admin/login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
    {
        var username = loginRequest.Username;
        var password = loginRequest.Password;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            return BadRequest(ErrorMessages.EMPTYCREDENTIALS);

        var isAuthenticated = await _adminService.LoginAsync(username, password);

        if (!isAuthenticated) return BadRequest(ErrorMessages.INVALIDCREDENTIALS);

        var token = _jwtService.GenerateToken(username, "Admin");

        return Ok(new { username, token });
    }
}
