namespace Application.Common.Models;

public record ApplicationDto(
    long Id,
    long ServiceId,
    string ServiceName,
    long ApplicantId,
    string ApplicantName,
    string Status,
    int CurrentStep,
    DateTime? ScheduledDateTime,
    DateTime CreatedAt,
    List<StepSubmissionDto> Submissions
);

public record StepSubmissionDto(
    long Id,
    long StepId,
    string StepName,
    int SubmissionVersion,
    string? FormData,
    List<string> FilePaths,
    string Status,
    DateTime SubmittedAt,
    string? RejectionReason
);
