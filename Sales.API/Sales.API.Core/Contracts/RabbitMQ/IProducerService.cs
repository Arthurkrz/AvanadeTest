namespace Sales.API.Core.Contracts.RabbitMQ
{
    public interface IProducerService
    {
        Task PublishProductSale(int saleCode, int productCode, int soldAmount);
    }
}
