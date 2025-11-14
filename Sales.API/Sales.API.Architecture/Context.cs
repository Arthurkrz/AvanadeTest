using Microsoft.EntityFrameworkCore;
using Sales.API.Architecture.Configurations;
using Sales.API.Core.Entities;

namespace Sales.API.Architecture
{
    public class Context : DbContext
    {
        public DbSet<Sale> Sales { get; set; } = default!;

        public Context(DbContextOptions<Context> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SaleConfiguration).Assembly);
        }
    }
}
