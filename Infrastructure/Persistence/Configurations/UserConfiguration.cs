namespace Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Identity;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).ValueGeneratedOnAdd();

        builder.Property(u => u.Username).IsRequired().HasMaxLength(100);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(255);
        builder.Property(u => u.PasswordHash).IsRequired().HasMaxLength(255);

        builder.Property(u => u.Role)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(u => u.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(u => u.EmailConfirmed).IsRequired();
        builder.Property(u => u.PhotoPath).HasMaxLength(500);
        builder.Property(u => u.Phone).HasMaxLength(20);
        builder.Property(u => u.PermanentAddress).HasMaxLength(500);
        builder.Property(u => u.CurrentAddress).HasMaxLength(500);
        builder.Property(u => u.Year).HasMaxLength(50);
        builder.Property(u => u.Session).HasMaxLength(50);
        builder.Property(u => u.Semester).HasMaxLength(50);
        builder.Property(u => u.Faculty).HasMaxLength(100);
        builder.Property(u => u.Department).HasMaxLength(100);
        builder.Property(u => u.EmailVerificationToken).HasMaxLength(100);

        builder.HasIndex(u => u.Email).IsUnique();
        builder.HasIndex(u => u.Username).IsUnique();

        builder.Property(u => u.Version).IsConcurrencyToken();
        builder.Property(u => u.CreatedBy).IsRequired();
        builder.Property(u => u.CreatedAt).IsRequired();
    }
}
