using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sales.API.Architecture;
using Sales.API.Core.Common;
using Sales.API.Core.Contracts.Repository;
using Sales.API.Core.Contracts.Service;
using Sales.API.Core.Entities;
using Sales.API.Core.Enum;
using Sales.API.IOC;
using Sales.API.Tests.Integration.Utilities;
using Sales.API.Web.Controllers;
using Sales.API.Web.DTOs;

namespace Sales.API.Tests.Integration
{
    public class SaleControllerIntegrationTests : IClassFixture<SalesApiFactory>
    {
        private readonly SaleController _sut;
        private readonly IServiceProvider _serviceProvider;
        private readonly Context _context;
        private readonly HttpClient _client;

        private readonly ISaleService _saleService;
        private readonly ISaleRepository _saleRepository;

        public SaleControllerIntegrationTests(SalesApiFactory factory)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            ServiceCollection services = new();

            services.InjectRepositories(config);
            services.InjectRabbitMQ(config);
            services.InjectServices();

            services.AddDbContext<Context>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            _serviceProvider = services.BuildServiceProvider();

            _context = _serviceProvider.GetRequiredService<Context>();
            _saleService = _serviceProvider.GetRequiredService<ISaleService>();
            _saleRepository = _serviceProvider.GetRequiredService<ISaleRepository>();

            _client = factory.CreateClient();

            _sut = new SaleController(_saleService);
        }

        [Fact]
        public void ProcessSale_ShouldProcessSale()
        {
            // Arrange
            Cleanup();

            var sale = new SaleDTO() { BuyerCPF = 1, ProductCode = 1, SellAmount = 1 };


        }

        [Theory]
        [InlineData(0, 1, 1)]
        [InlineData(-1, 1, 1)]
        [InlineData(1, 0, 1)]
        [InlineData(1, -1, 1)]
        [InlineData(1, 1, 0)]
        [InlineData(1, 1, -1)]
        [InlineData(0, 0, 0)]
        [InlineData(-1, -1, -1)]
        public void ProcessSale_ShouldReturnBadRequest_WhenSaleRequestFormatIsIncorrect(int buyerCpf, int productCode, int sellAmount)
        {
            // Arrange
            Cleanup();

            var sale = new SaleDTO() { BuyerCPF = buyerCpf, ProductCode = productCode, SellAmount = sellAmount };

            // Act
            var result = _sut.ProcessSale(sale);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(ErrorMessages.INCORRECTFORMAT, badRequest.Value);
            
        }

        [Fact]
        public void GetAllSales_ShouldReturnAllSales()
        {
            // Arrange
            Cleanup();

            var saleList = new List<Sale>
            {
                new Sale(3, 1, 1, SaleStatus.Completed),
                new Sale(1, 2, 1, SaleStatus.Completed),
                new Sale(1, 1, 3, SaleStatus.Rejected),
                new Sale(2, 1, 1, SaleStatus.Completed),
                new Sale(1, 1, 1, SaleStatus.Rejected),
            };

            foreach (var sale in saleList) _saleRepository.Add(sale);

            // Act
            var result = _sut.GetAllSales();

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var sales = Assert.IsAssignableFrom<IEnumerable<Sale>>(ok.Value);

            sales.Should().BeEquivalentTo(
                saleList, options => options.Excluding(s => s.ID));
        }

        [Fact]
        public void GetAllSales_ShouldReturnNotFound_WhenNoSalesExist()
        {
            Cleanup();

            // Act
            var result = _sut.GetAllSales();

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(ErrorMessages.NOSALESFOUND, notFound.Value);
        }

        [Fact]
        public void GetSalesByBuyerCPF_ShouldReturnSalesForGivenBuyerCPF()
        {
            // Arrange
            Cleanup();

            var saleList = new List<Sale>
            {
                new Sale(3, 1, 1, SaleStatus.Completed),
                new Sale(1, 2, 1, SaleStatus.Completed),
                new Sale(1, 1, 3, SaleStatus.Rejected),
                new Sale(2, 1, 1, SaleStatus.Completed),
                new Sale(2, 1, 1, SaleStatus.Rejected),
            };

            var expectedSales = new List<Sale>
            {
                new Sale(2, 1, 1, SaleStatus.Completed),
                new Sale(2, 1, 1, SaleStatus.Rejected)
            };

            // Act
            var result = _sut.GetSalesByBuyerCPF(2);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var sales = Assert.IsAssignableFrom<IEnumerable<Sale>>(ok.Value);

            sales.Should().BeEquivalentTo(
                expectedSales, options => options.Excluding(s => s.ID));
        }

        [Fact]
        public void GetSalesByBuyerCPF_ShouldReturnNotFound_WhenBuyerNotFound()
        {
            // Arrange
            Cleanup();

            // Act
            var result = _sut.GetSalesByBuyerCPF(9999);

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(ErrorMessages.NOSALESFOUND, notFound.Value);
        }

        [Fact]
        public void GetSalesByProductCode_ShouldReturnSalesForGivenProductCode()
        {
            // Arrange
            Cleanup();

            var saleList = new List<Sale>
            {
                new Sale(3, 1, 1, SaleStatus.Completed),
                new Sale(1, 2, 1, SaleStatus.Completed),
                new Sale(1, 1, 3, SaleStatus.Rejected),
                new Sale(2, 2, 1, SaleStatus.Completed),
                new Sale(2, 3, 1, SaleStatus.Rejected),
            };

            var expectedSales = new List<Sale>
            {
                new Sale(1, 2, 1, SaleStatus.Completed),
                new Sale(2, 2, 1, SaleStatus.Completed)
            };

            // Act
            var result = _sut.GetSalesByProductCode(2);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var sales = Assert.IsAssignableFrom<IEnumerable<Sale>>(ok.Value);

            sales.Should().BeEquivalentTo(
                expectedSales, options => options.Excluding(s => s.ID));
        }

        [Fact]
        public void GetSalesByProductCode_ShouldReturnNotFound_WhenProductNotFound()
        {
            // Arrange
            Cleanup();

            // Act
            var result = _sut.GetSalesByProductCode(9999);

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(ErrorMessages.NOSALESFOUND, notFound.Value);
        }

        [Fact]
        public void GetByCode_ShouldReturnSaleForGivenSaleCode()
        {
            // Arrange
            Cleanup();

            _context.Database.ExecuteSqlRaw($"INSERT INTO Sales (" +
                $"BuyerCPF, ProductCode, SellAmount, Status, ID, SaleCode) " +
                $"VALUES (1, 1, 1, 1, {Guid.NewGuid()}, 1234)");

            // Act
            var result = _sut.GetByCode(1234);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var sale = Assert.IsAssignableFrom<Sale>(ok.Value);

            Assert.Equal(1, sale.BuyerCPF);
            Assert.Equal(1, sale.ProductCode);
            Assert.Equal(1, sale.SellAmount);
            Assert.Equal(SaleStatus.Completed, sale.Status);
            Assert.Equal(1234, sale.SaleCode);
        }

        [Fact]
        public void GetByCode_ShouldReturnNotFound_WhenSaleNotFound()
        {
            // Arrange
            Cleanup();

            // Act
            var result = _sut.GetByCode(9999);

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(ErrorMessages.SALENOTFOUND, notFound.Value);
        }

        private void Cleanup() => 
            _context.Database.ExecuteSqlRaw("TRUNCATE TABLE Sales");
    }
}