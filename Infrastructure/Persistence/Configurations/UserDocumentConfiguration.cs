namespace Infrastructure.Persistence.Configurations;

using Domain.Documents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class UserDocumentConfiguration : IEntityTypeConfiguration<UserDocument>
{
    public void Configure(EntityTypeBuilder<UserDocument> builder)
    {
        builder.ToTable("user_documents");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Id)
            .HasColumnName("Id")
            .ValueGeneratedOnAdd();

        builder.Property(d => d.UserId)
            .HasColumnName("UserId")
            .IsRequired();

        builder.Property(d => d.ApplicationId)
            .HasColumnName("ApplicationId");

        builder.Property(d => d.FileName)
            .HasColumnName("FileName")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(d => d.OriginalFileName)
            .HasColumnName("OriginalFileName")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(d => d.FilePath)
            .HasColumnName("FilePath")
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(d => d.FileType)
            .HasColumnName("FileType")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(d => d.FileSize)
            .HasColumnName("FileSize")
            .IsRequired();

        builder.Property(d => d.IsDeleted)
            .HasColumnName("IsDeleted")
            .IsRequired();

        builder.Property(d => d.Version)
            .HasColumnName("Version")
            .IsConcurrencyToken();

        builder.Property(d => d.CreatedBy)
            .HasColumnName("CreatedBy")
            .IsRequired();

        builder.Property(d => d.CreatedAt)
            .HasColumnName("CreatedAt")
            .IsRequired();

        builder.Property(d => d.ModifiedBy)
            .HasColumnName("ModifiedBy");

        builder.Property(d => d.ModifiedAt)
            .HasColumnName("ModifiedAt");

        builder.HasIndex(d => d.UserId);
        builder.HasIndex(d => d.ApplicationId);
        builder.HasIndex(d => d.IsDeleted);
    }
}
