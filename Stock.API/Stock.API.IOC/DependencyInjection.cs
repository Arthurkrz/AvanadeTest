using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Stock.API.Architecture.Repositories;
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
        }

        public static void InjectRepositories(this IServiceCollection services)
        {
            services.AddSingleton<IProductRepository, ProductRepository>();
        }

        public static void InjectValidators(this IServiceCollection services)
        {
            services.AddSingleton<IValidator<Product>, ProductValidator>();
        }
    }
}
