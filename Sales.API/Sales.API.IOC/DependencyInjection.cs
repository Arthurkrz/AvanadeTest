using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sales.API.Architecture;
using Sales.API.Architecture.Repositories;
using Sales.API.Core.Contracts.Repository;
using Sales.API.Core.Contracts.Service;
using Sales.API.Service;

namespace Sales.API.IOC
{
    public static class DependencyInjection
    {
        public static void InjectServices(IServiceCollection services)
        {
            services.AddScoped<ISaleService, SaleService>();
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
            // RabbitMQ logic
            return services;
        }
    }
}
