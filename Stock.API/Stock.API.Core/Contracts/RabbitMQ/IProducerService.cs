namespace Stock.API.Core.Contracts.RabbitMQ
{
    public interface IProducerService
    {
        Task PublishSaleProcessed(int saleCode, int productCode, IList<string> errors = null!);
    }
}
