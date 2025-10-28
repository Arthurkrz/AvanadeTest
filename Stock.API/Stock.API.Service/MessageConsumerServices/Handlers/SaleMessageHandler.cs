using Stock.API.Core.Contracts.Handler;
using Stock.API.Core.Contracts.Service;
using Stock.API.Service.MessageConsumerServices.Models;
using System.Text.Json;

namespace Stock.API.Service.MessageConsumerServices.Handlers
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
            _productService.UpdateStock(message!.ProductID, message.SoldAmount);
        }
    }
}
