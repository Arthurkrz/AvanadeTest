namespace Stock.API.Core.Contracts.RabbitMQ
{
    public interface IProducerService
    {
        Task PublishSaleProcessedAsync(int saleCode, int productCode, IList<string> errors = null!);
    }
}
