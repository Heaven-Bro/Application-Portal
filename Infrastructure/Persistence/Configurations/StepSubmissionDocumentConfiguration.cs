namespace Infrastructure.Persistence.Configurations;

using Domain.Documents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class StepSubmissionDocumentConfiguration : IEntityTypeConfiguration<StepSubmissionDocument>
{
    public void Configure(EntityTypeBuilder<StepSubmissionDocument> builder)
    {
        builder.ToTable("step_submission_documents");
        
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        
        builder.Property(x => x.StepSubmissionId).IsRequired();
        builder.Property(x => x.UserDocumentId).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        
        builder.HasOne(x => x.StepSubmission)
            .WithMany(s => s.Documents)
            .HasForeignKey(x => x.StepSubmissionId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(x => x.UserDocument)
            .WithMany(d => d.Submissions)
            .HasForeignKey(x => x.UserDocumentId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasIndex(x => x.StepSubmissionId);
        builder.HasIndex(x => x.UserDocumentId);
    }
}
