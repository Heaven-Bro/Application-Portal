namespace Domain.Applications;

using Domain.Common.Base;
using Domain.Common.Enums;
using Domain.Common.Exceptions;
using Domain.Documents;

public sealed class StepSubmission : Entity
{
    public long ApplicationId { get; private set; }
    public long StepId { get; private set; }
    public string? FormData { get; private set; }
    public string? FilePaths { get; private set; }
    public StepSubmissionStatus Status { get; private set; }
    public long? ReviewedBy { get; private set; }
    public DateTime? ReviewedAt { get; private set; }
    public string? RejectionReason { get; private set; }
    public bool IsLatest { get; private set; }

    private readonly List<StepSubmissionDocument> _documents = new();
    public IReadOnlyCollection<StepSubmissionDocument> Documents => _documents.AsReadOnly();

    private StepSubmission() { }

    internal static StepSubmission Create(long applicationId, long stepId, 
        string? formData, List<string> filePaths, long submittedBy)
    {
        var submission = new StepSubmission
        {
            ApplicationId = applicationId,
            StepId = stepId,
            FormData = formData,
            FilePaths = string.Join(";", filePaths),
            Status = StepSubmissionStatus.Pending,
            IsLatest = true
        };
        
        submission.MarkAsCreated(submittedBy);
        return submission;
    }

    internal static StepSubmission CreateWithDocuments(long applicationId, long stepId, 
        string? formData, List<long> documentIds, long submittedBy)
    {
        var submission = new StepSubmission
        {
            ApplicationId = applicationId,
            StepId = stepId,
            FormData = formData,
            FilePaths = null, 
            Status = StepSubmissionStatus.Pending,
            IsLatest = true
        };
        
        if (documentIds != null && documentIds.Any())
        {
            foreach (var docId in documentIds)
            {
                submission._documents.Add(StepSubmissionDocument.Create(docId));
            }
        }
        
        submission.MarkAsCreated(submittedBy);
        return submission;
}


    public void Approve(long adminId)
    {
        if (Status == StepSubmissionStatus.Approved)
            throw new InvalidStateTransitionException(Status.ToString(), "Approve");

        Status = StepSubmissionStatus.Approved;
        ReviewedBy = adminId;
        ReviewedAt = DateTime.UtcNow;
        MarkAsModified(adminId);
    }

    internal void Reject(string reason, long adminId)
    {
        if (Status == StepSubmissionStatus.Approved)
            throw new InvalidStateTransitionException(Status.ToString(), "Reject");

        Status = StepSubmissionStatus.Rejected;
        RejectionReason = reason;
        ReviewedBy = adminId;
        ReviewedAt = DateTime.UtcNow;
        IsLatest = false;
        MarkAsModified(adminId);
    }

    internal void MarkAsNotLatest()
    {
        IsLatest = false;
    }

    public List<string> GetFilePaths()
    {
        if (string.IsNullOrWhiteSpace(FilePaths))
            return new List<string>();

        return FilePaths.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList();
    }
}
