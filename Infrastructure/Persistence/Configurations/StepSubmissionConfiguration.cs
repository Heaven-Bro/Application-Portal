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

        builder.Property(ss => ss.FormData).HasColumnType("json");
        builder.Property(ss => ss.FilePaths).HasMaxLength(2000);
        builder.Property(ss => ss.RejectionReason).HasMaxLength(1000);

        builder.HasIndex(ss => new { ss.ApplicationId, ss.StepId, ss.IsLatest });
        builder.HasIndex(ss => ss.Status);
    }
}
