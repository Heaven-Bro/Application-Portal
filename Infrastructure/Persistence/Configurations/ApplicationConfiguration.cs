namespace Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Applications;

public class ApplicationConfiguration : IEntityTypeConfiguration<Application>
{
    public void Configure(EntityTypeBuilder<Application> builder)
    {
        builder.ToTable("applications");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).ValueGeneratedOnAdd();

        builder.HasMany(a => a.Submissions)
            .WithOne()
            .HasForeignKey(ss => ss.ApplicationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(a => new { a.ApplicantId, a.Status });
        builder.HasIndex(a => a.CreatedAt);
    }
}
