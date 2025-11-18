using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Stock.API.Core.Contracts.RabbitMQ;
using Stock.API.Service.RabbitMQ.Shared.Configurations;
using Stock.API.Service.RabbitMQ.Shared.Models;
using System.Text;
using System.Text.Json;

namespace Stock.API.Service.RabbitMQ.ProducerServices
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

            _factory = new ConnectionFactory
            {
                HostName = _settings.HostName,
                Port = _settings.Port,
                UserName = _settings.UserName,
                Password = _settings.Password,
                VirtualHost = _settings.VirtualHost
            };
        }

        public async Task PublishSaleProcessed(int saleCode, int productCode, IList<string> errors = null!)
        {
            _connection = await _factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            await _channel.ExchangeDeclareAsync("sales-exchange", ExchangeType.Direct, durable: true);

            if (errors is not null && errors.Count > 0)
            {
                var saleProcessedDTOWithErrors = new SaleProcessedDTO(saleCode, productCode, errors);
                Publish(saleProcessedDTOWithErrors, "sales-exchange", "sale.processed");
            }
            else
            {
                var saleProcessedDTO = new SaleProcessedDTO(saleCode, productCode);
                Publish(saleProcessedDTO, "sales-exchange", "sale.processed");
            }
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
