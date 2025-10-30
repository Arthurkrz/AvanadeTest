namespace Identity.API.Architecture.Configurations
{
    public class AdminConfiguration : IEntityTypeConfiguration<Admin>
    {
        public void Configure(EntityTypeBuilder<Admin> builder)
        {
            builder.HasKey(x => x.ID);

            builder.Property(x => x.ID)
                   .HasColumnType("uniqueidentifier")
                   .IsRequired();

            builder.Property(x => x.Username)
                   .HasMaxLength(20)
                   .IsRequired();

            builder.Property(x => x.Name)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(x => x.CPF)
                   .HasMaxLength(11)
                   .IsRequired();

            builder.HasIndex(x => x.Username)
                   .IsUnique();

            builder.HasIndex(x => x.Name)
                   .IsUnique();

            builder.HasIndex(x => x.CPF)
                   .IsUnique();

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

            builder.Property(x => x.FailedLoginCount);

            builder.Property(x => x.LockoutEnd)
                   .HasColumnType("datetime2");

            builder.Property(x => x.LastLoginDate)
                   .HasColumnType("datetime2")
                   .IsRequired();

            builder.Property(x => x.CreationDate)
                   .HasColumnType("datetime2")
                   .IsRequired();

            builder.Property(x => x.LastModifiedDate)
                   .HasColumnType("datetime2");

            builder.ToTable("Administrators");
        }
    }
}
