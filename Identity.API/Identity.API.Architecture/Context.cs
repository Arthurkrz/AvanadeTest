using Identity.API.Architecture.Configurations;
using Identity.API.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Identity.API.Architecture
{
    public class Context : DbContext
    {
        public DbSet<Admin> Admins { get; set; } = default!;

        public DbSet<Buyer> Buyers { get; set; } = default!;

        public Context(DbContextOptions<Context> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AdminConfiguration).Assembly);
        }
    }
}
