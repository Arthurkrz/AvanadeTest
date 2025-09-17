using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stock.API.Core.Entities;

namespace Stock.API.Architecture.Configurations
{
    internal class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(x => x.ID);

            builder.Property(x => x.ID)
                   .HasColumnType("uniqueidentifier")
                   .IsRequired();

            builder.Property(x => x.Name)
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(x => x.Description)
                    .HasMaxLength(1000);

            builder.Property(x => x.Price)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(x => x.AmountInStock)
                   .IsRequired();

            builder.Property(p => p.CreationDate)
                   .HasColumnType("datetime2")
                   .IsRequired();

            builder.Property(p => p.LastModifiedDate)
                   .HasColumnType("datetime2");

            builder.ToTable("Products");
        }
    }
}
