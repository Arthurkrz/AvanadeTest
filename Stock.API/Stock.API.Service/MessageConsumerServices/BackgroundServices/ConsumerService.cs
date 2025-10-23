using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Stock.API.Service.MessageConsumerServices.Configurations;
using Stock.API.Service.MessageConsumerServices.Constants;
using Stock.API.Service.MessageConsumerServices.Models;
using System.Text.Json;

namespace Stock.API.Service.MessageConsumerServices.BackgroundServices
{
    public class ConsumerService : BackgroundService
    {
        private readonly RabbitMQSettings _settings;
        private readonly ILogger<ConsumerService> _logger;

        private IConnection? _connection;
        private IChannel? _channel;

        private const string ExchangeName = "sales-exchange";

        public ConsumerService(IOptions<RabbitMQSettings> settings, ILogger<ConsumerService> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory
            {
                HostName = _settings.HostName,
                Port = _settings.Port,
                UserName = _settings.UserName,
                Password = _settings.Password,
                VirtualHost = _settings.VirtualHost
            };

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            await CreateDeadLetterQueue();
            await CreateRetryQueue();
            await CreateMainQueue();

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (sender, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = JsonSerializer.Deserialize<MessageDTO>(body);

                    if (message?.Content.Contains("error") == true)
                    {
                        await _channel.BasicPublishAsync(
                            exchange: "",
                            routingKey: QueueNames.RetryQueue,
                            body: ea.Body
                        );

                        await _channel.BasicAckAsync(ea.DeliveryTag, false);
                    }
                }
                catch (Exception ex)
                {

                    Console.WriteLine("Error processing message: " + ex.Message);
                    await _channel.BasicNackAsync(ea.DeliveryTag, false, requeue: false);
                }

                await _channel.BasicAckAsync(ea.DeliveryTag, false);
                Console.WriteLine("Consuming main queue...");
            };

            await _channel.BasicConsumeAsync(
                queue: QueueNames.MainQueue,
                autoAck: false,
                consumer: consumer
            );

            Console.WriteLine("Waiting messages...");
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        private async Task CreateMainQueue()
        {
            var mainArgs = new Dictionary<string, object?>
            {
                { "x-dead-letter-exchange", "" },
                { "x-dead-letter-routing-key", QueueNames.DeadLetterQueue }
            };

            await _channel!.QueueDeclareAsync(
                queue: QueueNames.MainQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: mainArgs
            );
        }

        private async Task CreateRetryQueue()
        {
            var retryArgs = new Dictionary<string, object?>
            {
                {"x-dead-letter-exchange", "" },
                { "x-dead-letter-routing-key", QueueNames.MainQueue },
                { "x-message-ttl", 10000 }
            };

            await _channel!.QueueDeclareAsync(
                queue: QueueNames.RetryQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: retryArgs
            );
        }

        private async Task CreateDeadLetterQueue()
        {
            await _channel!.QueueDeclareAsync(
                queue: QueueNames.DeadLetterQueue,
                durable: true,
                exclusive: false,
                autoDelete: false
            );
        }

        private async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_channel != null)
                await _channel.CloseAsync();

            if (_connection != null)
                await _connection.CloseAsync();

            await base.StopAsync(cancellationToken);
        }
    }
}
