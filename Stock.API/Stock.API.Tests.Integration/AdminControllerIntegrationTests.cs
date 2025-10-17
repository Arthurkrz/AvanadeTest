using FluentValidation;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Stock.API.Architecture;
using Stock.API.Core.Contracts.Repository;
using Stock.API.Core.Contracts.Service;
using Stock.API.IOC;
using Stock.API.Web.Controllers;
using Stock.API.Web.DTOs;

namespace Stock.API.Tests.Integration
{
    public class AdminControllerIntegrationTests
    {
        private readonly AdminController _sut;
        private readonly IServiceProvider _serviceProvider;
        private readonly Context _context;

        private readonly IValidator<AdminDTO> _adminDTOValidator;
        private readonly IValidator<RegisterRequest> _requestValidator;

        private readonly IAdminService _adminService;
        private readonly IAdminRepository _adminRepository;
        private readonly IPasswordHasher _passwordHasher;

        public AdminControllerIntegrationTests()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            ServiceCollection services = new();

            services.InjectRepositories(config);
            services.InjectServices();
            services.InjectValidators();

            _serviceProvider = services.BuildServiceProvider();
            _context = _serviceProvider.GetRequiredService<Context>();

            _adminDTOValidator = _serviceProvider.GetRequiredService<IValidator<AdminDTO>>();
            _requestValidator = _serviceProvider.GetRequiredService<IValidator<RegisterRequest>>();
            _adminService = _serviceProvider.GetRequiredService<IAdminService>();
            _adminRepository = _serviceProvider.GetRequiredService<IAdminRepository>();
            _passwordHasher = _serviceProvider.GetRequiredService<IPasswordHasher>();

            _sut = new AdminController(_adminService, _adminDTOValidator);
        }

        [Fact]
        public void Register_ShouldCreateAndInsertNewAdmin()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void Register_ShouldReturnBadRequest_WhenInvalidAdminDTO()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void Login_ShouldReturnOk_WhenValidCredentials()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void Login_ShouldReturnBadRequest_WhenEmptyCredentials()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void Login_ShouldReturnBadRequest_WhenInvalidCredentials()
        {
            throw new NotImplementedException();
        }
    }
}