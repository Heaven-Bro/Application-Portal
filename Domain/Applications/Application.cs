namespace Domain.Applications;

using Domain.Common.Base;
using Domain.Common.Enums;
using Domain.Common.Exceptions;

public sealed class Application : Entity
{
    public long ServiceId { get; private set; }
    public int ServiceVersion { get; private set; }
    public long ApplicantId { get; private set; }
    public ApplicationStatus Status { get; private set; }
    public int CurrentStep { get; private set; }
    public DateTime? ScheduledDateTime { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    private readonly List<StepSubmission> _submissions = new();
    public IReadOnlyCollection<StepSubmission> Submissions => _submissions.AsReadOnly();

    private Application() { }

    public static Application Create(long serviceId, int serviceVersion, long applicantId)
    {
        var application = new Application
        {
            ServiceId = serviceId,
            ServiceVersion = serviceVersion,
            ApplicantId = applicantId,
            Status = ApplicationStatus.Draft,
            CurrentStep = 1
        };
        
        application.MarkAsCreated(applicantId);
        return application;
    }

    public StepSubmission SubmitStep(long stepId, string? formData, List<string> filePaths)
    {
        if (Status == ApplicationStatus.Completed)
            throw new InvalidStateTransitionException(Status.ToString(), "SubmitStep");

        if (Status == ApplicationStatus.Rejected)
            throw new InvalidStateTransitionException(Status.ToString(), "SubmitStep");

        var existingSubmission = _submissions
            .Where(s => s.StepId == stepId)
            .OrderByDescending(s => s.SubmissionVersion)
            .FirstOrDefault();

        int version = existingSubmission?.SubmissionVersion + 1 ?? 1;

        var submission = StepSubmission.Create(Id, stepId, version, formData, filePaths, ApplicantId);
        _submissions.Add(submission);

        if (Status == ApplicationStatus.Draft)
            Status = ApplicationStatus.Submitted;

        MarkAsModified(ApplicantId);
        return submission;
    }

    public void ApproveStep(long submissionId, long adminId)
    {
        var submission = _submissions.FirstOrDefault(s => s.Id == submissionId);
        if (submission == null)
            throw new BusinessRuleViolationException("Submission not found");

        submission.Approve(adminId);
        Status = ApplicationStatus.InReview;
        MarkAsModified(adminId);
    }

    public void RejectStep(long submissionId, string reason, long adminId)
    {
        var submission = _submissions.FirstOrDefault(s => s.Id == submissionId);
        if (submission == null)
            throw new BusinessRuleViolationException("Submission not found");

        submission.Reject(reason, adminId);
        MarkAsModified(adminId);
    }

    public void MarkAsApproved(DateTime scheduledDateTime, long adminId)
    {
        if (Status != ApplicationStatus.InReview)
            throw new InvalidStateTransitionException(Status.ToString(), "MarkAsApproved");

        Status = ApplicationStatus.Approved;
        ScheduledDateTime = scheduledDateTime;
        MarkAsModified(adminId);
    }

    public void MarkAsCompleted(long adminId)
    {
        if (Status != ApplicationStatus.Approved)
            throw new InvalidStateTransitionException(Status.ToString(), "MarkAsCompleted");

        Status = ApplicationStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        MarkAsModified(adminId);
    }

    public void MarkAsRejected(string reason, long adminId)
    {
        Status = ApplicationStatus.Rejected;
        MarkAsModified(adminId);
    }
}
