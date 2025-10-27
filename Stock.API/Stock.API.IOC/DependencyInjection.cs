using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Stock.API.Architecture;
using Stock.API.Architecture.Repositories;
using Stock.API.Core.Contracts.Handler;
using Stock.API.Core.Contracts.Repository;
using Stock.API.Core.Contracts.Service;
using Stock.API.Core.Entities;
using Stock.API.Core.Validators;
using Stock.API.Service;
using Stock.API.Service.MessageConsumerServices.BackgroundServices;
using Stock.API.Service.MessageConsumerServices.Configurations;
using Stock.API.Service.MessageConsumerServices.Handlers;

namespace Stock.API.IOC
{
    public static class DependencyInjection
    {
        public static void InjectServices(this IServiceCollection services)
        {
            services.AddScoped<IProductService, ProductService>();
        }

        public static void InjectRepositories(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<Context>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            services.AddScoped<IProductRepository, ProductRepository>();
        }

        public static void InjectValidators(this IServiceCollection services)
        {
            services.AddScoped<IValidator<Product>, ProductValidator>();
        }

        public static IServiceCollection InjectRabbitMQ(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<RabbitMQSettings>(config.GetSection("RabbitMQ"));
            services.AddScoped<IMessageHandler, SaleMessageHandler>();
            services.AddHostedService<ConsumerService>();
            return services;
        }

        public static IServiceCollection InjectHandlers(this IServiceCollection services)
        {
            services.AddScoped<IMessageHandler, SaleMessageHandler>();
            return services;
        }
    }
}
