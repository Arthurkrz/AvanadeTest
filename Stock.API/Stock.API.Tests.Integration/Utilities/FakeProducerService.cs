using Stock.API.Core.Contracts.RabbitMQ;

namespace Stock.API.Tests.Integration.Utilities
{
    public class FakeProducerService : IProducerService
    {
        public Task PublishSaleProcessedAsync(int saleCode, int productCode, IList<string> errors = null!)
        {
            return Task.CompletedTask;
        }
    }
}
