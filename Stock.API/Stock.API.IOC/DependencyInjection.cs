using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Stock.API.Architecture;
using Stock.API.Architecture.Repositories;
using Stock.API.Core.Common;
using Stock.API.Core.Contracts.Repository;
using Stock.API.Core.Contracts.Service;
using Stock.API.Core.Entities;
using Stock.API.Core.Validators;
using Stock.API.Service;

namespace Stock.API.IOC
{
    public static class DependencyInjection
    {
        public static void InjectServices(this IServiceCollection services)
        {
            services.AddSingleton<IProductService, ProductService>();
            services.AddSingleton<IAdminService, AdminService>();
            services.AddSingleton<IPasswordHasher, PasswordHasher>();
        }

        public static void InjectRepositories(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<Context>(options => 
                options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            services.AddSingleton<IProductRepository, ProductRepository>();
            services.AddSingleton<IAdminRepository, AdminRepository>();
        }

        public static void InjectValidators(this IServiceCollection services)
        {
            services.AddSingleton<IValidator<Product>, ProductValidator>();
            services.AddSingleton<IValidator<RegisterRequest>, RegisterRequestValidator>();
        }
    }
}
