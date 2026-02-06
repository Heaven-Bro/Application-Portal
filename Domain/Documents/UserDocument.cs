namespace Domain.Documents;

using Domain.Common.Base;
using Domain.Common.Exceptions;

public sealed class UserDocument : Entity
{
    public long UserId { get; private set; }
    public long? ApplicationId { get; private set; }
    public string FileName { get; private set; } = string.Empty;
    public string OriginalFileName { get; private set; } = string.Empty;
    public string FilePath { get; private set; } = string.Empty;
    public string FileType { get; private set; } = string.Empty;
    public long FileSize { get; private set; }
    public bool IsDeleted { get; private set; }

    private readonly List<StepSubmissionDocument> _submissions = new();
    public IReadOnlyCollection<StepSubmissionDocument> Submissions => _submissions.AsReadOnly();

    private UserDocument() { }

    public static UserDocument Create(
        long userId,
        long? applicationId,
        string fileName,
        string originalFileName,
        string filePath,
        string fileType,
        long fileSize,
        long createdBy)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new BusinessRuleViolationException("File name cannot be empty");

        if (string.IsNullOrWhiteSpace(filePath))
            throw new BusinessRuleViolationException("File path cannot be empty");

        if (fileSize <= 0)
            throw new BusinessRuleViolationException("File size must be greater than zero");

        var document = new UserDocument
        {
            UserId = userId,
            ApplicationId = applicationId,
            FileName = fileName,
            OriginalFileName = originalFileName,
            FilePath = filePath,
            FileType = fileType,
            FileSize = fileSize,
            IsDeleted = false
        };

        document.MarkAsCreated(createdBy);
        return document;
    }

    public void SoftDelete(long deletedBy)
    {
        if (IsDeleted)
            throw new InvalidStateTransitionException("Document already deleted", "SoftDelete");

        IsDeleted = true;
        MarkAsModified(deletedBy);
    }

    public void Restore(long restoredBy)
    {
        if (!IsDeleted)
            throw new InvalidStateTransitionException("Document not deleted", "Restore");

        IsDeleted = false;
        MarkAsModified(restoredBy);
    }
}
