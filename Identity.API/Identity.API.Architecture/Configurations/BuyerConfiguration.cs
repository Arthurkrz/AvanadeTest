using Identity.API.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.API.Architecture.Configurations
{
    public class BuyerConfiguration : IEntityTypeConfiguration<Buyer>
    {
        public void Configure(EntityTypeBuilder<Buyer> builder)
        {
            builder.HasKey(x => x.ID);

            builder.Property(x => x.ID)
                   .HasColumnType("uniqueidentifier")
                   .IsRequired();

            builder.Property(x => x.Username)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(x => x.Name)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(x => x.CPF)
                   .HasMaxLength(11)
                   .IsRequired();

            builder.Property(x => x.Email)
                   .HasMaxLength(80)
                   .IsRequired();

            builder.HasIndex(x => x.Username)
                   .IsUnique();

            builder.HasIndex(x => x.Name)
                   .IsUnique();

            builder.HasIndex(x => x.CPF)
                   .IsUnique();

            builder.HasIndex(x => x.Email)
                   .IsUnique();

            builder.Property(x => x.PhoneNumber)
                   .HasMaxLength(15)
                   .IsRequired();

            builder.Property(x => x.DeliveryAddress)
                   .HasMaxLength(200)
                   .IsRequired();

            builder.Property(x => x.PasswordHash)
                   .HasColumnType("varbinary(256)")
                   .IsRequired();

            builder.Property(x => x.PasswordSalt)
                   .HasColumnType("varbinary(128)")
                   .IsRequired();

            builder.Property(x => x.HashAlgorithm)
                   .HasMaxLength(20)
                   .IsRequired();

            builder.Property(x => x.HashParams)
                   .HasMaxLength(200)
                   .IsRequired();

            builder.ToTable("Buyers");
        }
    }
}
