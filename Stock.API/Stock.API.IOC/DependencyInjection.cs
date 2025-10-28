using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Stock.API.Architecture;
using Stock.API.Architecture.Repositories;
using Stock.API.Core.Common;
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
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IAdminService, AdminService>();
        }

        public static void InjectRepositories(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<Context>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IAdminRepository, AdminRepository>();
        }

        public static void InjectValidators(this IServiceCollection services)
        {
            services.AddScoped<IValidator<Product>, ProductValidator>();
            services.AddScoped<IValidator<RegisterRequest>, RegisterRequestValidator>();
        }

        public static IServiceCollection InjectRabbitMQ(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<RabbitMQSettings>(config.GetSection("RabbitMQ"));

            services.Scan(scan => scan
                .FromAssemblyOf<SaleMessageHandler>()
                .AddClasses(classes => classes.AssignableTo<IMessageHandler>())
                .AsSelfWithInterfaces()
                .WithScopedLifetime());

            services.AddHostedService<ConsumerService>();
            return services;
        }
    }
}
