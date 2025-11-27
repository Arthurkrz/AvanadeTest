using Stock.API.Core.Contracts.Handler;
using Stock.API.Core.Contracts.Service;
using Stock.API.Service.RabbitMQ.Shared.Models;
using System.Text.Json;

namespace Stock.API.Service.RabbitMQ.ConsumerServices.Handlers
{
    public class SaleMessageHandler : IMessageHandler
    {
        private readonly IProductService _productService;

        public SaleMessageHandler(IProductService productService)
        {
            _productService = productService;
        }

        public async Task HandleAsync(string messageJson)
        {
            var message = JsonSerializer.Deserialize<ProductSaleDTO>(messageJson);
            await _productService.UpdateStockAsync(message!.SaleCode, message!.ProductCode, message.SoldAmount);
        }
    }
}
