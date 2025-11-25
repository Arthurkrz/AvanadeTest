using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sales.API.Core.Contracts.Repository;

namespace Sales.API.Service.BackgroundServices
{
    public class SaleExpirationWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SaleExpirationWorker> _logger;

        public SaleExpirationWorker(IServiceProvider serviceProvider, ILogger<SaleExpirationWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Sale Expiration Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var saleRepository = scope.ServiceProvider.GetRequiredService<ISaleRepository>();

                    var pendingSales = await saleRepository.GetPendingSalesAsync(TimeSpan.FromSeconds(10));

                    foreach(var saleCode in pendingSales)
                    {
                        await saleRepository.UpdateStatusAsync(saleCode, Core.Enum.SaleStatus.Expired);
                        _logger.LogWarning($"Sale with code {saleCode} has been marked as expired due to no response.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in Sale Expiration Service.");
                }
                
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }
}
