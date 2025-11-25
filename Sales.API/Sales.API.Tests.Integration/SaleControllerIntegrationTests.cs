using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sales.API.Architecture;
using Sales.API.Core.Common;
using Sales.API.Core.Entities;
using Sales.API.Core.Enum;
using Sales.API.Tests.Integration.Utilities;
using Sales.API.Web.DTOs;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Sales.API.Tests.Integration
{
    public class SaleControllerIntegrationTests : IClassFixture<SalesApiFactory>
    {
        private readonly SalesApiFactory _factory;
        private readonly HttpClient _client;

        public SaleControllerIntegrationTests(SalesApiFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        private async Task Cleanup()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<Context>();
            await db.Database.ExecuteSqlRawAsync("TRUNCATE TABLE Sales");
            await db.SaveChangesAsync();
        }

        [Fact]
        public async Task ProcessSale_ShouldProcessSale()
        {
            // Arrange
            await Cleanup();

            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add("X-Test-Role", "Buyer");

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Test");

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<Context>();

            var dto = new SaleDTO() { BuyerCPF = 1, ProductCode = 1, SellAmount = 1 };

            // Act
            var response = await client.PostAsJsonAsync("/api/sales/processSale", dto);

            // Assert
            response.EnsureSuccessStatusCode();

            var sale = await response.Content.ReadFromJsonAsync<Sale>();
            var sales = await db.Sales.ToListAsync();

            Assert.NotEqual(Guid.Empty, sale!.ID);
            Assert.NotEqual(0, sale.SaleCode);
            Assert.Equal(1, sale.BuyerCPF);
            Assert.Equal(1, sale.ProductCode);
            Assert.Equal(1, sale.SellAmount);
            Assert.Equal(SaleStatus.Pending, sale.Status);

            Assert.Single(sales);
            var addedSale = sales.First();

            Assert.NotEqual(Guid.Empty, addedSale.ID);
            Assert.NotEqual(0, addedSale.SaleCode);
            Assert.Equal(1, addedSale.BuyerCPF);
            Assert.Equal(1, addedSale.ProductCode);
            Assert.Equal(1, addedSale.SellAmount);
            Assert.Equal(SaleStatus.Pending, addedSale.Status);
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
        public async Task ProcessSale_ShouldReturnBadRequest_WhenSaleRequestFormatIsIncorrect(int buyerCpf, int productCode, int sellAmount)
        {
            // Arrange
            await Cleanup();

            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add("X-Test-Role", "Buyer");

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Test");

            var dto = new SaleDTO() { BuyerCPF = buyerCpf, ProductCode = productCode, SellAmount = sellAmount };

            // Act
            var response = await client.PostAsJsonAsync("/api/sales/processSale", dto);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var msg = await response.Content.ReadAsStringAsync();
            Assert.Contains(ErrorMessages.INCORRECTFORMAT, msg);
        }

        [Fact]
        public async Task GetAllSales_ShouldReturnAllSales()
        {
            // Arrange
            await Cleanup();

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<Context>();

            var saleList = new List<Sale>
            {
                new Sale(3, 1, 1, SaleStatus.Completed) { SaleCode = 1 },
                new Sale(1, 2, 1, SaleStatus.Completed) { SaleCode = 2 },
                new Sale(1, 1, 3, SaleStatus.Rejected) { SaleCode = 3 },
                new Sale(2, 1, 1, SaleStatus.Completed) { SaleCode = 4 },
                new Sale(1, 1, 1, SaleStatus.Rejected) { SaleCode = 5 },
            };

            await db.Sales.AddRangeAsync(saleList);
            await db.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/sales/all");

            // Assert
            response.EnsureSuccessStatusCode();

            var returned = await response.Content.ReadFromJsonAsync<IEnumerable<Sale>>();

            Assert.NotNull(returned);
            Assert.Equal(5, returned.Count());
        }

        [Fact]
        public async Task GetAllSales_ShouldReturnNotFound_WhenNoSalesExist()
        {
            // Arrange
            await Cleanup();

            // Act
            var response = await _client.GetAsync("/api/sales/all");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            var msg = await response.Content.ReadAsStringAsync();
            Assert.Contains(ErrorMessages.NOSALESFOUND, msg);
        }

        [Fact]
        public async Task GetSalesByBuyerCPF_ShouldReturnSalesForGivenBuyerCPF()
        {
            // Arrange
            await Cleanup();

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<Context>();

            var saleList = new List<Sale>
            {
                new Sale(3, 1, 1, SaleStatus.Completed) { SaleCode = 1 },
                new Sale(1, 2, 1, SaleStatus.Completed) { SaleCode = 2 },
                new Sale(1, 1, 3, SaleStatus.Rejected) { SaleCode = 3 },
                new Sale(2, 1, 1, SaleStatus.Completed) { SaleCode = 4 },
                new Sale(2, 1, 1, SaleStatus.Rejected) { SaleCode = 5 },
            };

            await db.Sales.AddRangeAsync(saleList);
            await db.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/sales/buyer/2");

            // Assert
            response.EnsureSuccessStatusCode();
            var sales = await response.Content.ReadFromJsonAsync<IEnumerable<Sale>>();

            Assert.NotNull(sales);
            Assert.Equal(2, sales.Count());
        }

        [Fact]
        public async Task GetSalesByBuyerCPF_ShouldReturnNotFound_WhenBuyerNotFound()
        {
            // Arrange
            await Cleanup();

            // Act
            var response = await _client.GetAsync("/api/sales/buyer/9999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetSalesByProductCode_ShouldReturnSalesForGivenProductCode()
        {
            // Arrange
            await Cleanup();

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<Context>();

            var saleList = new List<Sale>
            {
                new Sale(3, 1, 1, SaleStatus.Completed) { SaleCode = 1},
                new Sale(1, 2, 1, SaleStatus.Completed) { SaleCode = 2},
                new Sale(1, 1, 3, SaleStatus.Rejected) { SaleCode = 3},
                new Sale(2, 2, 1, SaleStatus.Completed) { SaleCode = 4 },
                new Sale(2, 3, 1, SaleStatus.Rejected) { SaleCode = 5 },
            };

            await db.Sales.AddRangeAsync(saleList);
            await db.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/sales/product/2");

            // Assert
            response.EnsureSuccessStatusCode();
            var sales = await response.Content.ReadFromJsonAsync<IEnumerable<Sale>>();

            Assert.Equal(2, sales!.Count());
        }

        [Fact]
        public async Task GetSalesByProductCode_ShouldReturnNotFound_WhenProductNotFound()
        {
            // Arrange
            await Cleanup();

            // Act
            var response = await _client.GetAsync("/api/sales/product/999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetByCode_ShouldReturnSaleForGivenSaleCode()
        {
            // Arrange
            await Cleanup();

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<Context>();

            var sale = new Sale(1, 1, 1, SaleStatus.Completed)
            {
                SaleCode = 1234
            };

            await db.Sales.AddAsync(sale);
            await db.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/sales/1234");

            // Assert
            response.EnsureSuccessStatusCode();

            var returned = await response.Content.ReadFromJsonAsync<Sale>();

            Assert.NotNull(returned);
            Assert.Equal(1234, returned.SaleCode);
        }

        [Fact]
        public async Task GetByCode_ShouldReturnNotFound_WhenSaleNotFound()
        {
            // Arrange
            await Cleanup();

            // Act
            var response = await _client.GetAsync("/api/sales/9999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}