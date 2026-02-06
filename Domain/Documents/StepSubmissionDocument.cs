namespace Domain.Documents;

using Domain.Applications;

public sealed class StepSubmissionDocument
{
    public long Id { get; private set; }
    public long StepSubmissionId { get; private set; }
    public long UserDocumentId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public StepSubmission StepSubmission { get; private set; } = null!;
    public UserDocument UserDocument { get; private set; } = null!;

    private StepSubmissionDocument() { }

    internal static StepSubmissionDocument Create(long userDocumentId)
    {
        return new StepSubmissionDocument
        {
            UserDocumentId = userDocumentId,
            CreatedAt = DateTime.UtcNow
        };
    }
}
