using FluentValidation;
using Identity.API.Architecture;
using Identity.API.Core.Common;
using Identity.API.Core.Contracts.Repository;
using Identity.API.Core.Contracts.Service;
using Identity.API.Core.Entities;
using Identity.API.Core.Validators;
using Identity.API.IOC;
using Identity.API.Tests.Integration.Utilities;
using Identity.API.Web.Controllers;
using Identity.API.Web.DTOs;
using Identity.API.Web.Validators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.API.Tests.Integration
{
    public class BuyerControllerIntegrationTests
    {
        private readonly BuyerController _sut;
        private readonly IServiceProvider _serviceProvider;
        private readonly Context _context;
        private readonly BuyerTestTableManager _buyerTestTableManager;

        private readonly IValidator<BuyerDTO> _buyerDTOValidator;

        private readonly IBuyerService _buyerService;
        private readonly IBaseRepository<Buyer> _buyerRepository;

        public BuyerControllerIntegrationTests()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            ServiceCollection services = new();

            services.AddValidatorsFromAssemblyContaining<BuyerDTOValidator>();
            services.AddValidatorsFromAssemblyContaining<UserRegisterRequestValidator>();

            services.InjectRepositories(config);
            services.InjectServices();
            services.InjectValidators();

            services.AddDbContext<Context>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            _serviceProvider = services.BuildServiceProvider();

            _context = _serviceProvider.GetRequiredService<Context>();
            _buyerDTOValidator = _serviceProvider.GetRequiredService<IValidator<BuyerDTO>>();
            _buyerService = _serviceProvider.GetRequiredService<IBuyerService>();
            _buyerRepository = _serviceProvider.GetRequiredService<IBaseRepository<Buyer>>();

            _buyerTestTableManager = new BuyerTestTableManager(_context, _buyerRepository);
            _sut = new BuyerController(_buyerService, _buyerDTOValidator);
        }

        [Fact]
        public void Register_ShouldCreateAndInsertNewBuyer()
        {
            // Arrange
            _buyerTestTableManager.Cleanup();

            var buyerDTO = new BuyerDTO()
            {
                Username = "NewBuyer",
                Name = "Buyer Name",
                CPF = "12345678900",
                Password = "SecurePassword123!",
                Email = "Email@Email.com",
                PhoneNumber = "21998795324",
                DeliveryAddress = "Delivery Address 123"
            };

            // Act
            var result = _sut.Register(buyerDTO);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var anon = ok.Value!;

            var actual = new
            {
                Username = (string)anon.GetType().GetProperty("Username")!.GetValue(anon, null)!
            };

            Assert.Equal("NewBuyer", actual.Username);

            Buyer? addedBuyer = _buyerRepository.GetByUsername("NewBuyer");

            Assert.NotNull(addedBuyer);
            Assert.Equal(addedBuyer.Username, actual.Username);
        }

        [Fact]
        public void Register_ShouldReturnBadRequest_WhenInvalidBuyerDTO()
        {
            // Arrange
            _buyerTestTableManager.Cleanup();

            // Act
            var result = _sut.Register(new BuyerDTO());

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);

            Assert.Equal("The request is invalid: " +
                         "Username must not be null or empty, " +
                         "Name must not be null or empty, " +
                         "CPF must not be null or empty, " +
                         "Password must not be null or empty, " +
                         "Email must not be null or empty, " +
                         "Phone Number must not be null or empty, " +
                         "Delivery Address must not be null or empty", badRequest.Value);
        }

        [Fact]
        public void Login_ShouldReturnOk_WhenValidCredentials()
        {
            // Arrange
            _buyerTestTableManager.Cleanup();
            _buyerTestTableManager.InsertBuyer();
            var loginRequest = new LoginRequest
            { Password = "Password0", Username = "Username0" };

            // Act
            var result = _sut.Login(loginRequest);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Username0", ok.Value);
        }

        [Fact]
        public void Login_ShouldReturnBadRequest_WhenEmptyCredentials()
        {
            // Arrange
            _buyerTestTableManager.Cleanup();
            var loginRequest = new LoginRequest { Password = "", Username = "" };

            // Act
            var result = _sut.Login(loginRequest);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(ErrorMessages.EMPTYCREDENTIALS, badRequest.Value);
        }

        [Fact]
        public void Login_ShouldReturnBadRequest_WhenInvalidCredentials()
        {
            // Arrange
            _buyerTestTableManager.Cleanup();
            _buyerTestTableManager.InsertBuyer();
            var loginRequest = new LoginRequest
            { Password = "WrongPassword", Username = "Username0" };

            // Act
            var result = _sut.Login(loginRequest);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(ErrorMessages.INVALIDCREDENTIALS, badRequest.Value);
        }
    }
}
