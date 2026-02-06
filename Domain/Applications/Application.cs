namespace Domain.Applications;

using Domain.Common.Base;
using Domain.Common.Enums;
using Domain.Common.Exceptions;

public sealed class Application : Entity
{
    public long ServiceId { get; private set; }
    public long ApplicantId { get; private set; }
    public ApplicationStatus Status { get; private set; }
    public int CurrentStep { get; private set; }
    public DateTime? ScheduledDateTime { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string? AdminNotes { get; private set; }

    private readonly List<StepSubmission> _submissions = new();
    public IReadOnlyCollection<StepSubmission> Submissions => _submissions.AsReadOnly();

    private Application() { }

    public static Application Create(long serviceId, long applicantId)
    {
        var application = new Application
        {
            ServiceId = serviceId,
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

        var existingSubmissions = _submissions.Where(s => s.StepId == stepId).ToList();
        foreach (var oldSubmission in existingSubmissions)
        {
            oldSubmission.MarkAsNotLatest();
        }

        var submission = StepSubmission.Create(Id, stepId, formData, filePaths, ApplicantId);
        _submissions.Add(submission);

        if (Status == ApplicationStatus.Draft)
            Status = ApplicationStatus.Submitted;

        MarkAsModified(ApplicantId);
        return submission;
    }

    public StepSubmission SubmitStepWithDocuments(long stepId, string? formData, List<long> documentIds)
    {
        if (Status == ApplicationStatus.Completed)
            throw new InvalidStateTransitionException(Status.ToString(), "SubmitStep");

        if (Status == ApplicationStatus.Rejected)
            throw new InvalidStateTransitionException(Status.ToString(), "SubmitStep");

        var existingSubmissions = _submissions.Where(s => s.StepId == stepId).ToList();
        foreach (var oldSubmission in existingSubmissions)
        {
            oldSubmission.MarkAsNotLatest();
        }

        var submission = StepSubmission.CreateWithDocuments(Id, stepId, formData, documentIds, ApplicantId);
        _submissions.Add(submission);

        if (Status == ApplicationStatus.Draft)
            Status = ApplicationStatus.Submitted;

        MarkAsModified(ApplicantId);
        return submission;
    }

    public void ApproveStep(long submissionId, long adminId, bool stepRequiresApproval)
    {
        var submission = _submissions.FirstOrDefault(s => s.Id == submissionId);
        if (submission == null)
            throw new BusinessRuleViolationException("Submission not found");

        submission.Approve(adminId);
        
        if (stepRequiresApproval)
        {
            Status = ApplicationStatus.InReview;
        }

        MarkAsModified(adminId);
    }

    public void RejectStep(long submissionId, string reason, long adminId)
    {
        var submission = _submissions.FirstOrDefault(s => s.Id == submissionId);
        if (submission == null)
            throw new BusinessRuleViolationException("Submission not found");

        submission.Reject(reason, adminId);
        
        Status = ApplicationStatus.Rejected;
        AdminNotes = reason;
        
        MarkAsModified(adminId);
    }


    public void MarkAsApproved(DateTime scheduledDateTime, long adminId, string? notes = null)
    {
        if (Status != ApplicationStatus.InReview && Status != ApplicationStatus.Submitted)
            throw new InvalidStateTransitionException(Status.ToString(), "MarkAsApproved");

        Status = ApplicationStatus.Approved;
        ScheduledDateTime = scheduledDateTime;
        
        if (!string.IsNullOrWhiteSpace(notes))
            AdminNotes = notes;
        
        MarkAsModified(adminId);
    }

    public void MarkAsCompleted(long adminId, string? notes = null)
    {
        if (Status != ApplicationStatus.Approved)
            throw new InvalidStateTransitionException(Status.ToString(), "MarkAsCompleted");

        Status = ApplicationStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        
        if (!string.IsNullOrWhiteSpace(notes))
            AdminNotes = notes;
        
        MarkAsModified(adminId);
    }

    public void MarkAsRejected(string reason, long adminId)
    {
        Status = ApplicationStatus.Rejected;
        AdminNotes = reason;
        MarkAsModified(adminId);
    }

    public void AdvanceStep()
    {
        CurrentStep++;
        MarkAsModified(ApplicantId);
    }

    public void UpdateAdminNotes(string notes, long adminId)
    {
        AdminNotes = notes;
        MarkAsModified(adminId);
    }

    public bool CanApplicantModify()
    {
        return Status != ApplicationStatus.Completed && Status != ApplicationStatus.Rejected;
    }
}
