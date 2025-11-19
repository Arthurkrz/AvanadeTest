using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sales.API.Architecture;
using Sales.API.Architecture.Repositories;
using Sales.API.Core.Contracts.Client;
using Sales.API.Core.Contracts.Handler;
using Sales.API.Core.Contracts.RabbitMQ;
using Sales.API.Core.Contracts.Repository;
using Sales.API.Core.Contracts.Service;
using Sales.API.Service;
using Sales.API.Service.Clients;
using Sales.API.Service.RabbitMQ.MessageConsumerServices.BackgroundServices;
using Sales.API.Service.RabbitMQ.MessageConsumerServices.Handlers;
using Sales.API.Service.RabbitMQ.MessageProducerServices;
using Sales.API.Service.RabbitMQ.Shared.Configurations;

namespace Sales.API.IOC
{
    public static class DependencyInjection
    {
        public static IServiceCollection InjectServices(this IServiceCollection services)
        {
            services.AddScoped<ISaleService, SaleService>();
            services.AddScoped<IProducerService, ProducerService>();

            return services;
        }

        public static IServiceCollection InjectRepositories(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<Context>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            services.AddScoped<ISaleRepository, SaleRepository>();

            return services;
        }

        public static IServiceCollection InjectRabbitMQ(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<RabbitMQSettings>(config.GetSection("RabbitMQ"));

            services.Scan(scan => scan
                .FromAssemblyOf<SaleStatusMessageHandler>()
                .AddClasses(classes => classes.AssignableTo<IMessageHandler>())
                .AsSelfWithInterfaces()
                .WithScopedLifetime());

            services.AddHostedService<ConsumerService>();

            return services;
        }

        public static IServiceCollection InjectHttpClients(this IServiceCollection services, IConfiguration config)
        {
            services.AddHttpClient<IStockClient, StockClient>(client =>
            {
                client.BaseAddress = new Uri(config["Services:StockAPI"]!);
            });

            services.AddHttpClient<IIdentityClient, IdentityClient>(client =>
            {
                client.BaseAddress = new Uri(config["Services:IdentityAPI"]!);
            });

            return services;
        }
    }
}
