using FluentValidation;
using Identity.API.Core.Common;
using Identity.API.Core.Contracts.Service;
using Identity.API.Web.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Web.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BuyerController : Controller
    {
        private readonly IBuyerService _buyerService;
        private readonly IValidator<BuyerDTO> _buyerDTOValidator;

        public BuyerController(IBuyerService buyerService, IValidator<BuyerDTO> buyerDTOValidator)
        {
            _buyerService = buyerService;
            _buyerDTOValidator = buyerDTOValidator;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(BuyerDTO buyerDTO)
        {
            var validationResult = _buyerDTOValidator.Validate(buyerDTO);

            if (!validationResult.IsValid) return BadRequest(ErrorMessages.INVALIDREQUEST
                .Replace("{error}", string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))));

            var request = new UserRegisterRequest(buyerDTO.Username!, buyerDTO.Name!, 
                                                  buyerDTO.CPF!, buyerDTO.Password!, 
                                                  buyerDTO.Email!, buyerDTO.PhoneNumber!, 
                                                  buyerDTO.DeliveryAddress!);

            var buyer = await _buyerService.RegisterAsync(request);

            return Ok(new { buyer.Username });
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            var username = loginRequest.Username;
            var password = loginRequest.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return BadRequest(ErrorMessages.EMPTYCREDENTIALS);

            var isAuthenticated = await _buyerService.LoginAsync(username, password);

            if (!isAuthenticated) return BadRequest(ErrorMessages.INVALIDCREDENTIALS);

            return Ok(username);
        }

        [Authorize(Roles = "SalesAPI")]
        [HttpGet("exists/{cpf}")]
        public async Task<bool> Exists(string cpf) =>
            await _buyerService.IsExistingByCPFAsync(cpf);
    }
}
