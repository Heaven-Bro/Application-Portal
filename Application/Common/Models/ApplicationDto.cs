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
    int StepOrder,
    string StepName,
    string? FormData,
    string Status,
    DateTime SubmittedAt,
    string? RejectionReason,
    List<DocumentDto> Documents
);

public record DocumentDto(
    long Id,
    string FileName,
    string FilePath
);
