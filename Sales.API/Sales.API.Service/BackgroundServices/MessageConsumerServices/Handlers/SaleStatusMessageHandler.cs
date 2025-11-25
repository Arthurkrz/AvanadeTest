using Sales.API.Core.Contracts.Handler;
using Sales.API.Core.Contracts.Service;
using Sales.API.Service.RabbitMQ.Shared.Models;
using System.Text.Json;

namespace Sales.API.Service.BackgroundServices.MessageConsumerServices.Handlers
{
    public class SaleStatusMessageHandler : IMessageHandler
    {
        private readonly ISaleService _saleService;

        public SaleStatusMessageHandler(ISaleService saleService)
        {
            _saleService = saleService;
        }

        public async Task HandleAsync(string messageJson)
        {
            var message = JsonSerializer.Deserialize<SaleStatusDTO>(messageJson);
            await _saleService.UpdateSaleStatusAsync(message!.SaleCode, message.Success, message.Errors);
        }
    }
}
