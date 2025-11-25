using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sales.API.Architecture;
using Sales.API.Core.Contracts.Repository;
using Sales.API.Core.Entities;
using Sales.API.Core.Enum;
using Sales.API.Tests.Integration.Utilities;

namespace Sales.API.Tests.Integration
{
    public class SaleExpirationWorkerIntegrationTests : IClassFixture<SalesApiFactory>
    {
        private readonly SalesApiFactory _factory;

        public SaleExpirationWorkerIntegrationTests(SalesApiFactory factory) 
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
        public async Task ExecuteAsync_ShouldUpdatePendingSalesToExpired()
        {
            // Arrange
            await Cleanup();
            var client = _factory.CreateClient();

            var sale = new Sale(1, 1, 1, SaleStatus.Pending) { SaleCode = 1 };
            sale.CreatedAt = DateTime.UtcNow.AddSeconds(-20);

            using (var arrangeScope = _factory.Services.CreateScope())
            {
                var saleRepository = arrangeScope.ServiceProvider.GetRequiredService<ISaleRepository>();
                await saleRepository.AddAsync(sale);
            }

            // Act
            await Task.Delay(12000);

            // Assert
            using (var assertScope = _factory.Services.CreateScope())
            {
                var saleRepository = assertScope.ServiceProvider.GetRequiredService<ISaleRepository>();
                var updatedSale = await saleRepository.GetByCodeAsync(1);

                Assert.Equal(SaleStatus.Expired, updatedSale.Status);
            }
        }
    }
}
