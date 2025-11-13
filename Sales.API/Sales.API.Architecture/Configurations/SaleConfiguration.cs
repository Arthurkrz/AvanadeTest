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
            
            builder.Property(s => s.BuyerID)
                   .IsRequired();
            
            builder.Property(s => s.ProductID)
                   .IsRequired();
            
            builder.Property(s => s.SellAmount)
                   .IsRequired();

            builder.Property(s => s.Status)
                   .IsRequired();

            builder.HasIndex(s => s.ID)
                   .IsUnique();

            builder.HasIndex(s => s.BuyerID)
                   .IsUnique();

            builder.HasIndex(s => s.ProductID)
                   .IsUnique();

            builder.ToTable("Sales");
        }
    }
}
