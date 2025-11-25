using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sales.API.Core.Entities;

namespace Sales.API.Architecture.Configurations
{
    public class SaleConfiguration : IEntityTypeConfiguration<Sale>
    {
        public void Configure(EntityTypeBuilder<Sale> builder)
        {
            builder.HasKey(s => s.ID);
            
            builder.Property(s => s.BuyerCPF)
                   .IsRequired();
            
            builder.Property(s => s.ProductCode)
                   .IsRequired();
            
            builder.Property(s => s.SellAmount)
                   .IsRequired();

            builder.Property(s => s.Status)
                   .IsRequired();

            builder.Property(s => s.CreatedAt)
                   .IsRequired();

            builder.HasIndex(s => s.ID)
                   .IsUnique();

            builder.HasIndex(s => s.SaleCode)
                   .IsUnique();

            builder.ToTable("Sales");
        }
    }
}
