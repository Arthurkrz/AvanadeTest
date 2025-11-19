using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Sales.API.Core.Contracts.Handler;
using Sales.API.Core.Enum;
using Sales.API.Service.RabbitMQ.MessageConsumerServices.Handlers;
using Sales.API.Service.RabbitMQ.Shared.Configurations;
using Sales.API.Service.RabbitMQ.Shared.Constants;
using Sales.API.Service.RabbitMQ.Shared.Models;
using System.Text;
using System.Text.Json;

namespace Sales.API.Service.RabbitMQ.MessageConsumerServices.BackgroundServices
{
    public class ConsumerService : BackgroundService
    {
        private readonly RabbitMQSettings _settings;
        private readonly ILogger<ConsumerService> _logger;
        private readonly IServiceProvider _serviceProvider;

        private IConnection? _connection;
        private IChannel? _channel;

        private readonly Dictionary<string, Type> _handlers = new()
        {
            { "sale.processed", typeof(SaleStatusMessageHandler) }
        };
        
        public ConsumerService(RabbitMQSettings settings, ILogger<ConsumerService> logger, IServiceProvider serviceProvider)
        {
            _settings = settings;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory()
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

            await _channel.ExchangeDeclareAsync("sales-exchange", ExchangeType.Direct, durable: true);
            await _channel.QueueBindAsync(QueueNames.MainQueue, "sales-exchange", "sale.processed");

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += OnMessageReceivedAsync;

            await _channel.BasicConsumeAsync(
                queue: QueueNames.MainQueue,
                autoAck: false,
                consumer: consumer
            );

            _logger.LogInformation("RabbitMQ consumer started.");
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        private async Task OnMessageReceivedAsync(object sender, BasicDeliverEventArgs ea)
        {
            try
            {
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                var message = JsonSerializer.Deserialize<SaleStatusDTO>(json);

                if (message is null || message.SaleCode <= 0 || message.ProductCode <= 0)
                {
                    _logger.LogWarning($"Invalid message received: {json}");
                    await _channel!.BasicNackAsync(ea.DeliveryTag, false, requeue: false);
                    return;
                }

                if (_handlers.TryGetValue(ea.RoutingKey, out var handlerType))
                {
                    using var scope = _serviceProvider.CreateScope();
                    var handler = (IMessageHandler)scope.ServiceProvider.GetRequiredService(handlerType);
                    await handler.HandleAsync(json);

                    await _channel!.BasicAckAsync(ea.DeliveryTag, false);

                    var status = message.Success ? "Success" : "Failure";

                    _logger.LogInformation($"Processed sale status update for sale {message.SaleCode} with status {status}.");
                }
                else
                {
                    _logger.LogWarning($"No handler found for routing key: {ea.RoutingKey}");
                    await _channel!.BasicNackAsync(ea.DeliveryTag, false, requeue: false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing message: {ex.Message}");

                await _channel!.BasicPublishAsync(
                    exchange: "",
                    routingKey: QueueNames.RetryQueue,
                    body: ea.Body
                );

                await _channel!.BasicAckAsync(ea.DeliveryTag, false);
            }
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
                { "x-dead-letter-exchange", "" },
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
    }
}
