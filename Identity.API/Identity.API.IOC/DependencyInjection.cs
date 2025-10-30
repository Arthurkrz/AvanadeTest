namespace Identity.API.IOC
{
    public class DependencyInjection
    {
        public static IServiceCollection InjectServices(this IServiceCollection services)
        {
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IAdminService, AdminService>();
        }

        public static IServiceCollection InjectRepositories(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<Context>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            services.AddScoped<IAdminRepository, AdminRepository>();
        }
    }
}
