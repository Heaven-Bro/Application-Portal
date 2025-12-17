namespace Domain.Applications;

using Domain.Common.Base;
using Domain.Common.Enums;
using Domain.Common.Exceptions;

public sealed class StepSubmission : Entity
{
    public long ApplicationId { get; private set; }
    public long StepId { get; private set; }
    public int SubmissionVersion { get; private set; }
    public string? FormData { get; private set; }
    public string? FilePaths { get; private set; }
    public StepSubmissionStatus Status { get; private set; }
    public long? ReviewedBy { get; private set; }
    public DateTime? ReviewedAt { get; private set; }
    public string? RejectionReason { get; private set; }
    public bool IsLatest { get; private set; }

    private StepSubmission() { }

    internal static StepSubmission Create(long applicationId, long stepId, int version, 
        string? formData, List<string> filePaths, long submittedBy)
    {
        var submission = new StepSubmission
        {
            ApplicationId = applicationId,
            StepId = stepId,
            SubmissionVersion = version,
            FormData = formData,
            FilePaths = string.Join(";", filePaths),
            Status = StepSubmissionStatus.Pending,
            IsLatest = true
        };
        
        submission.MarkAsCreated(submittedBy);
        return submission;
    }

    internal void Approve(long adminId)
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

    public List<string> GetFilePaths()
    {
        if (string.IsNullOrWhiteSpace(FilePaths))
            return new List<string>();

        return FilePaths.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList();
    }
}
