using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Stock.API.Architecture;
using Stock.API.Core.Contracts.Handler;
using Stock.API.Core.Contracts.RabbitMQ;
using Stock.API.Core.Contracts.Repository;
using Stock.API.Core.Contracts.Service;
using Stock.API.IOC;
using Stock.API.Service;
using Stock.API.Service.RabbitMQ.ConsumerServices.Handlers;
using Stock.API.Service.RabbitMQ.Shared.Models;
using Stock.API.Tests.Integration.Utilities;
using System.Text.Json;

namespace Stock.API.Tests.Integration
{
    public class SaleMessageHandlerIntegrationTests
    {
        private readonly IMessageHandler _sut;
        private readonly ProductTestTableManager _productTestTableManager;
        private readonly IServiceProvider _serviceProvider;
        private readonly Context _context;

        private readonly IProductRepository _productRepository;

        public SaleMessageHandlerIntegrationTests()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            ServiceCollection services = new();

            services.InjectRepositories(config);

            services.AddTransient<IProducerService, FakeProducerService>();
            services.AddTransient<IProductService, ProductService>();
            services.AddTransient<IMessageHandler, SaleMessageHandler>();

            services.InjectValidators();

            services.AddDbContext<Context>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            _serviceProvider = services.BuildServiceProvider();

            _context = _serviceProvider.GetRequiredService<Context>();
            _productRepository = _serviceProvider.GetRequiredService<IProductRepository>();

            _productTestTableManager = new ProductTestTableManager(_context, _productRepository);

            _sut = _serviceProvider.GetRequiredService<IMessageHandler>();
        }

        [Fact]
        public async Task HandleAsync_ShouldProcessSaleMessage_AndUpdateProductStock()
        {
            // Arrange
            _productTestTableManager.Cleanup();
            await _productTestTableManager.InsertProductAsync();

            var dto = new ProductSaleDTO
            {
                SaleCode = 1234,
                ProductCode = 1,
                SoldAmount = 5
            };

            var json = JsonSerializer.Serialize(dto);

            // Act
            await _sut.HandleAsync(json);

            // Assert
            var updatedProduct = await _productRepository.GetByCodeAsync(dto.ProductCode);
            Assert.NotNull(updatedProduct);
            Assert.Equal(5, updatedProduct.AmountInStock);
        }
    }
}
