using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sales.API.Architecture;
using Sales.API.Core.Common;
using Sales.API.Core.Entities;
using Sales.API.Core.Enum;
using Sales.API.Service.BackgroundServices.MessageConsumerServices.Handlers;
using Sales.API.Service.RabbitMQ.Shared.Models;
using Sales.API.Tests.Integration.Utilities;
using System.Text.Json;

namespace Sales.API.Tests.Integration
{
    public class ConsumerServiceIntegrationTests : IClassFixture<SalesApiFactory>
    {
        private readonly SalesApiFactory _factory;

        public ConsumerServiceIntegrationTests(SalesApiFactory factory)
        {
            _factory = factory;
        }

        private async Task Cleanup()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<Context>();
            await db.Database.ExecuteSqlRawAsync("TRUNCATE TABLE Sales");
        } 

        [Fact]
        public async Task OnMessageReceivedAsync_ShouldProcessSaleStatusResponseAndUpdateWithRejected()
        {
            // Arrange
            await Cleanup();

            using var scope = _factory.Services.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<SaleStatusMessageHandler>();
            var db = scope.ServiceProvider.GetRequiredService<Context>();

            var sale = new Sale(1, 1, 1, SaleStatus.Pending)
            {
                SaleCode = 1234
            };

            await db.Sales.AddAsync(sale);
            await db.SaveChangesAsync();

            var dto = new SaleStatusDTO
            {
                SaleCode = 1234,
                Success = false,
                Errors = ["OUT_OF_STOCK"]
            };

            var json = JsonSerializer.Serialize(dto);

            // Act
            var saex = await Assert.ThrowsAsync<SaleApiException>(() => 
                handler.HandleAsync(json));

            // Assert
            var updatedSale = await db.Sales.FirstOrDefaultAsync(s => s.SaleCode == 1234);

            Assert.NotNull(updatedSale);
            Assert.Equal(1234, updatedSale?.SaleCode);
            Assert.Equal(SaleStatus.Rejected, updatedSale?.Status);
        }

        [Fact]
        public async Task OnMessageReceivedAsync_ShouldProcessSaleStatusResponseAndUpdateWithCompleted()
        {
            // Arrange
            await Cleanup();

            using var scope = _factory.Services.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<SaleStatusMessageHandler>();
            var db = scope.ServiceProvider.GetRequiredService<Context>();

            var sale = new Sale(1, 1, 1, SaleStatus.Pending)
            {
                SaleCode = 5678
            };

            await db.Sales.AddAsync(sale);
            await db.SaveChangesAsync();

            var dto = new SaleStatusDTO
            {
                SaleCode = 5678,
                Success = true,
                Errors = []
            };

            var json = JsonSerializer.Serialize(dto);

            // Act
            await handler.HandleAsync(json);

            // Assert
            var updatedSale = await db.Sales.FirstOrDefaultAsync(s => s.SaleCode == 5678);

            Assert.NotNull(updatedSale);
            Assert.Equal(5678, updatedSale?.SaleCode);
            Assert.Equal(SaleStatus.Completed, updatedSale?.Status);
        }
    }
}
