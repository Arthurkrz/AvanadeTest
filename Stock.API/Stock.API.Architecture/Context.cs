using Microsoft.EntityFrameworkCore;
using Stock.API.Architecture.Configurations;
using Stock.API.Core.Entities;

namespace Stock.API.Architecture
{
    public class Context : DbContext
    {
        public DbSet<Product> Products { get; set; } = default!;

        public Context(DbContextOptions<Context> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
        }

        public override int SaveChanges()
        {
            foreach (var entry in ChangeTracker.Entries<Entity>())
            {
                if (entry.State == EntityState.Modified)
                    entry.Entity.LastModifiedDate = DateTime.Now;
            }

            return base.SaveChanges();
        }
    }
}
