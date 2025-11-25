using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Sales.API.Core.Contracts.RabbitMQ;
using Sales.API.Service.RabbitMQ.Shared.Configurations;
using Sales.API.Service.RabbitMQ.Shared.Models;
using System.Text;
using System.Text.Json;

namespace Sales.API.Service.RabbitMQ.MessageProducerServices
{
    public class ProducerService : IProducerService
    {
        private readonly RabbitMQSettings _settings;
        private readonly ConnectionFactory _factory;

        private IConnection? _connection;
        private IChannel? _channel;

        public ProducerService(IOptions<RabbitMQSettings> settings)
        {
            _settings = settings.Value;

            _factory = new ConnectionFactory()
            {
                HostName = _settings.HostName,
                Port = _settings.Port,
                UserName = _settings.UserName,
                Password = _settings.Password,
                VirtualHost = _settings.VirtualHost
            };
        }

        public async Task PublishProductSale(int saleCode, int productCode, int soldAmount)
        {
            _connection = await _factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            await _channel.ExchangeDeclareAsync("sales-exchange", ExchangeType.Direct, durable: true);

            var dto = new ProductSaleDTO 
            { 
                SaleCode = saleCode, 
                ProductCode = productCode, 
                SoldAmount = soldAmount
            };

            Publish(dto, "sales-exchange", "product.sold");
        }

        private void Publish(GenericDTO DTO, string exchange, string routingKey)
        {
            if (DTO is null) throw new ArgumentNullException(nameof(DTO));

            var json = JsonSerializer.Serialize(DTO);
            var body = Encoding.UTF8.GetBytes(json);

            _channel!.BasicPublishAsync(
                exchange: exchange,
                routingKey: routingKey,
                body: body
            );
        }
    }
}
