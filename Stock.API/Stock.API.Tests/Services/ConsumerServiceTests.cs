using Castle.Core.Logging;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Stock.API.Core.Contracts.Handler;
using Stock.API.Service.MessageConsumerServices.BackgroundServices;
using Stock.API.Service.MessageConsumerServices.Configurations;
using Stock.API.Service.MessageConsumerServices.Constants;
using Stock.API.Service.MessageConsumerServices.Models;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Stock.API.Tests.Services
{
    public class ConsumerServiceTests
    {
        private readonly Mock<IServiceProvider> _serviceProviderMock = new();
        private readonly Mock<IServiceScope> _serviceScopeMock = new();
        private readonly Mock<IServiceScopeFactory> _scopeFactoryMock = new();
        private readonly Mock<IMessageHandler> _messageHandlerMock = new();
        private readonly Mock<IChannel> _channelMock = new();
        private readonly Mock<ILogger<ConsumerService>> _loggerMock = new();

        private readonly ConsumerService _sut;

        public ConsumerServiceTests()
        {
            _serviceScopeMock.Setup(s => s.ServiceProvider)
                             .Returns(new ServiceCollection()
                                .AddScoped(_ => _messageHandlerMock.Object)
                                .BuildServiceProvider());

            _scopeFactoryMock.Setup(f => f.CreateScope())
                .Returns(_serviceScopeMock.Object);

            _serviceProviderMock.Setup(p => p.GetService(typeof(IServiceScopeFactory)))
                                .Returns(_scopeFactoryMock.Object);

            var settings = new RabbitMQSettings
            {
                HostName = "localhost",
                Port = 5672,
                UserName = "guest",
                Password = "guest",
                VirtualHost = "/"
            };

            var options = Options.Create(settings);

            _sut = new ConsumerService(options, _loggerMock.Object, _serviceProviderMock.Object);

            typeof(ConsumerService)
                .GetField("_channel", BindingFlags.NonPublic | BindingFlags.Instance)!
                .SetValue(_sut, _channelMock.Object);

            typeof(ConsumerService)
                .GetField("_handlers", BindingFlags.NonPublic | BindingFlags.Instance)!
                .SetValue(_sut, new Dictionary<string, Type>
                { { "product.sold", typeof(IMessageHandler) } });
        }

        private static BasicDeliverEventArgs CreateEvent(string routingKey, string json, 
                                                         ulong tag = 1, string exchange = "", 
                                                         bool redelivered = false)
        {
            IBasicProperties? props = null;

            return new BasicDeliverEventArgs(
                consumerTag: "test-consumer",
                deliveryTag: tag,
                redelivered: redelivered,
                exchange: exchange,
                routingKey: routingKey,
                properties: props!,
                body: new ReadOnlyMemory<byte>(Encoding.UTF8.GetBytes(json))
            );
        }

        private static Task InvokeAsync(object sut, BasicDeliverEventArgs ea) =>
            (Task)sut.GetType()
                     .GetMethod("OnMessageReceivedAsync", BindingFlags.NonPublic | BindingFlags.Instance)!
                     .Invoke(sut, new object[] { sut, ea })!;

        [Fact]
        public async Task OnMessageReceivedAsync_ShouldInvokeHandlerAndAck()
        {
            // Arrange
            var dto = new ProductSaleDTO { ProductID = Guid.Parse("3b24869d-648e-4b5f-9c2c-82e8b07dc612"), SoldAmount = 10 };
            var json = JsonSerializer.Serialize(dto);
            var ea = CreateEvent("product.sold", json);

            // Act
            await InvokeAsync(_sut, ea);

            // Assert
            _messageHandlerMock.Verify(h => h.HandleAsync(json), Times.Once);
            _channelMock.Verify(c => c.BasicAckAsync(ea.DeliveryTag, false, It.IsAny<CancellationToken>()), Times.Once);
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Processed sale for product 3b24869d-648e-4b5f-9c2c-82e8b07dc612")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Theory]
        [MemberData(nameof(GetInvalidDTOs))]
        public async Task OnMessageReceivedAsync_ShouldLogWarningAndNack_WhenInvalidMessage(ProductSaleDTO dto)
        {
            // Arrange
            var json = JsonSerializer.Serialize(dto);
            var ea = CreateEvent("product.sold", json);

            // Act
            await InvokeAsync(_sut, ea);

            // Assert
            _messageHandlerMock.Verify(h => h.HandleAsync(json), Times.Never);
            _channelMock.Verify(c => c.BasicNackAsync(ea.DeliveryTag, false, false, It.IsAny<CancellationToken>()), Times.Once);
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Received null or invalid message.")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task OnMessageReceivedAsync_ShouldLogWarningAndNack_WhenNoHandlerFound()
        {
            // Arrange
            var dto = new ProductSaleDTO { ProductID = Guid.NewGuid(), SoldAmount = 10 };
            var json = JsonSerializer.Serialize(dto);
            var ea = CreateEvent("unknown.routing.key", json);

            // Act
            await InvokeAsync(_sut, ea);

            // Assert
            _channelMock.Verify(c => c.BasicNackAsync(ea.DeliveryTag, false, false, It.IsAny<CancellationToken>()), Times.Once);
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("No handler found for routing key")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task OnMessageReceivedAsync_ShouldLogErrorAckAndPublishToRetryQueue_WhenHandlerThrowsException()
        {
            // Arrange
            var dto = new ProductSaleDTO { ProductID = Guid.NewGuid(), SoldAmount = 10 };
            var json = JsonSerializer.Serialize(dto);
            var ea = CreateEvent("product.sold", json);

            _messageHandlerMock.Setup(h => h.HandleAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception("ERROR"));

            // Act
            await InvokeAsync(_sut, ea);

            // Assert
            var publishInvocation = _channelMock.Invocations
                .FirstOrDefault(i => i.Method.Name == nameof(IChannel.BasicPublishAsync));

            publishInvocation.Should().NotBeNull("a retry message should have been published");

            var args = publishInvocation!.Arguments;

            args[0].Should().Be("");
            args[1].Should().Be(QueueNames.RetryQueue);
            ((bool)args[2]).Should().BeFalse();
            args[4].Should().BeOfType<ReadOnlyMemory<byte>>();
            ((ReadOnlyMemory<byte>)args[4]).Span
                .SequenceEqual(ea.Body.Span)
                .Should().BeTrue("the same message body should be republished to the retry queue");

            _channelMock.Verify(c => c.BasicAckAsync(ea.DeliveryTag, false, It.IsAny<CancellationToken>()), Times.Once);

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error processing message:")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        public static IEnumerable<object[]> GetInvalidDTOs()
        {
            yield return new object[]
            {
                new ProductSaleDTO { ProductID = Guid.NewGuid(), SoldAmount = -5 }
            };

            yield return new object[]
            {
                new ProductSaleDTO { ProductID = Guid.Empty, SoldAmount = 5 }
            };

            yield return new object[]
            {
                new ProductSaleDTO { ProductID = Guid.Empty, SoldAmount = -5 }
            };
        }
    }
}
