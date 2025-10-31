using FluentValidation;
using Identity.API.Architecture;
using Identity.API.Architecture.Repositories;
using Identity.API.Core.Common;
using Identity.API.Core.Contracts.Repository;
using Identity.API.Core.Contracts.Service;
using Identity.API.Core.Validators;
using Identity.API.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.API.IOC
{
    public static class DependencyInjection
    {
        public static IServiceCollection InjectServices(this IServiceCollection services)
        {
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IAdminService, AdminService>();

            return services;
        }

        public static IServiceCollection InjectRepositories(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<Context>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            services.AddScoped<IAdminRepository, AdminRepository>();

            return services;
        }

        public static IServiceCollection InjectValidators(this IServiceCollection services)
        {
            services.AddScoped<IValidator<RegisterRequest>, RegisterRequestValidator>();

            return services;
        }
    }
}
