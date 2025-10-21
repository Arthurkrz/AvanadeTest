using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Stock.API.Architecture;
using Stock.API.Core.Common;
using Stock.API.Core.Contracts.Repository;
using Stock.API.Core.Contracts.Service;
using Stock.API.Core.Entities;
using Stock.API.Core.Validators;
using Stock.API.IOC;
using Stock.API.Tests.Integration.Utilities;
using Stock.API.Web.Controllers;
using Stock.API.Web.DTOs;
using Stock.API.Web.Validators;

namespace Stock.API.Tests.Integration
{
    public class AdminControllerIntegrationTests
    {
        private readonly AdminController _sut;
        private readonly IServiceProvider _serviceProvider;
        private readonly Context _context;
        private readonly AdminTestTableManager _adminTestTableManager;

        private readonly IValidator<AdminDTO> _adminDTOValidator;

        private readonly IAdminService _adminService;
        private readonly IAdminRepository _adminRepository;

        public AdminControllerIntegrationTests()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            ServiceCollection services = new();

            services.AddValidatorsFromAssemblyContaining<AdminDTOValidator>();
            services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();

            services.InjectRepositories(config);
            services.InjectServices();
            services.InjectValidators();

            services.AddDbContext<Context>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            _serviceProvider = services.BuildServiceProvider();

            _context = _serviceProvider.GetRequiredService<Context>();
            _adminDTOValidator = _serviceProvider.GetRequiredService<IValidator<AdminDTO>>();
            _adminService = _serviceProvider.GetRequiredService<IAdminService>();
            _adminRepository = _serviceProvider.GetRequiredService<IAdminRepository>();

            _adminTestTableManager = new AdminTestTableManager(_context, _adminRepository);
            _sut = new AdminController(_adminService, _adminDTOValidator);
        }

        [Fact]
        public void Register_ShouldCreateAndInsertNewAdmin()
        {
            // Arrange
            _adminTestTableManager.Cleanup();

            var adminDTO = new AdminDTO()
            {
                Username = "NewAdmin",
                Name = "Admin Name",
                CPF = "12345678900",
                Password = "SecurePassword123!"
            };

            // Act
            var result = _sut.Register(adminDTO);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var anon = ok.Value!;

            var actual = new
            {
                ID = (Guid)anon.GetType().GetProperty("ID")!.GetValue(anon)!,
                Username = (string)anon.GetType().GetProperty("Username")!.GetValue(anon)!
            };

            Assert.Equal("NewAdmin", actual.Username);

            Admin? addedAdmin = _adminRepository.GetByUsername("NewAdmin");

            Assert.NotNull(addedAdmin);
            Assert.Equal(addedAdmin.ID, actual.ID);
            Assert.Equal(addedAdmin.Username, actual.Username);
        }

        [Fact]
        public void Register_ShouldReturnBadRequest_WhenInvalidAdminDTO()
        {
            // Arrange
            _adminTestTableManager.Cleanup();

            // Act
            var result = _sut.Register(new AdminDTO());

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);

            Assert.Equal("The request is invalid: " +
                         "Username must not be null or empty, " +
                         "Name must not be null or empty, " +
                         "CPF must not be null or empty, " +
                         "Password must not be null or empty", badRequest.Value);
        }

        [Fact]
        public void Login_ShouldReturnOk_WhenValidCredentials()
        {
            // Arrange
            _adminTestTableManager.Cleanup();
            _adminTestTableManager.InsertAdmin();

            // Act
            var result = _sut.Login("Username0", "Password0");

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Username0", ok.Value);
        }

        [Fact]
        public void Login_ShouldReturnBadRequest_WhenEmptyCredentials()
        {
            // Arrange
            _adminTestTableManager.Cleanup();

            // Act
            var result = _sut.Login(string.Empty, string.Empty);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(ErrorMessages.EMPTYCREDENTIALS, badRequest.Value);
        }

        [Fact]
        public void Login_ShouldReturnBadRequest_WhenInvalidCredentials()
        {
            // Arrange
            _adminTestTableManager.Cleanup();
            _adminTestTableManager.InsertAdmin();

            // Act
            var result = _sut.Login("Username0", "WrongPassword");

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(ErrorMessages.INVALIDCREDENTIALS, badRequest.Value);
        }
    }
}