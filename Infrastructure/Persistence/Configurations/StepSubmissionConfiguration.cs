namespace Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Applications;

public class StepSubmissionConfiguration : IEntityTypeConfiguration<StepSubmission>
{
    public void Configure(EntityTypeBuilder<StepSubmission> builder)
    {
        builder.ToTable("step_submissions");

        builder.HasKey(ss => ss.Id);
        builder.Property(ss => ss.Id).ValueGeneratedOnAdd();

        builder.Property(ss => ss.ApplicationId).IsRequired();
        builder.Property(ss => ss.StepId).IsRequired();

        builder.Property(ss => ss.FormData).HasColumnType("json");
        builder.Property(ss => ss.FilePaths).HasMaxLength(2000);
        builder.Property(ss => ss.RejectionReason).HasMaxLength(1000);

        builder.Property(ss => ss.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(ss => ss.IsLatest).IsRequired();

        builder.HasIndex(ss => new { ss.ApplicationId, ss.StepId, ss.IsLatest });
        builder.HasIndex(ss => ss.Status);

        builder.Property(ss => ss.Version).IsConcurrencyToken();
        builder.Property(ss => ss.CreatedBy).IsRequired();
        builder.Property(ss => ss.CreatedAt).IsRequired();

        builder.HasMany(s => s.Documents)
            .WithOne(d => d.StepSubmission)
            .HasForeignKey(d => d.StepSubmissionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
