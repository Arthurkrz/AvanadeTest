namespace Identity.API.Architecture
{
    public class Context : DbContext
    {
        public DbSet<Admin> Admins { get; set; } = default!;

        public Context(DbContextOptions<Context> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AdminConfiguration());
        }
    }
}
