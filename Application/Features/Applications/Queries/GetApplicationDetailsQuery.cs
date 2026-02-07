namespace Application.Features.Applications.Queries;

using MediatR;
using Application.Common.Interfaces;
using Application.Common.Models;
using Shared.Contracts.Services;
using Shared.Contracts.Common;
using Domain.Repositories;

public sealed record GetApplicationDetailsQuery(long ApplicationId) : IRequest<Result<ApplicationDetailsDto>>;

public sealed class GetApplicationDetailsQueryHandler : IRequestHandler<GetApplicationDetailsQuery, Result<ApplicationDetailsDto>>
{
    private readonly IApplicationRepository _applicationRepository;
    private readonly IServiceRepository _serviceRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetApplicationDetailsQueryHandler(
        IApplicationRepository applicationRepository,
        IServiceRepository serviceRepository,
        IUserRepository userRepository,
        ICurrentUserService currentUserService)
    {
        _applicationRepository = applicationRepository;
        _serviceRepository = serviceRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<ApplicationDetailsDto>> Handle(GetApplicationDetailsQuery request, CancellationToken cancellationToken)
    {
        var application = await _applicationRepository.GetByIdWithSubmissionsAsync(request.ApplicationId, cancellationToken);
        
        if (application == null)
            return Result<ApplicationDetailsDto>.Failure("Application not found");

        var currentUserId = _currentUserService.UserId;
        
        if (application.ApplicantId != currentUserId)
            return Result<ApplicationDetailsDto>.Failure("Unauthorized");

        var service = await _serviceRepository.GetByIdWithStepsAsync(application.ServiceId, cancellationToken);
        
        if (service == null)
            return Result<ApplicationDetailsDto>.Failure("Service not found");

        var user = await _userRepository.GetByIdAsync(application.ApplicantId, cancellationToken);
        
        if (user == null)
            return Result<ApplicationDetailsDto>.Failure("User not found");

        var stepDetails = service.Steps.OrderBy(s => s.Order).Select(step =>
        {
            var submission = application.Submissions
                .Where(s => s.StepId == step.Id)
                .OrderByDescending(s => s.CreatedAt)
                .FirstOrDefault();

            return new StepDetailDto(
                step.Id,
                step.Name,
                step.Order,
                step.Instructions,
                step.DownloadableFormUrl,
                step.RequiresFileUpload,
                step.RequiresTextInput,
                step.IsOptional,
                step.RequiresApproval,
                ParseUploadConfig(step.UploadConfig),
                submission != null ? new StepSubmissionDto(
                    submission.Id,
                    submission.StepId,
                    step.Order,
                    step.Name,
                    submission.FormData,
                    submission.Status.ToString(),
                    submission.CreatedAt,
                    submission.RejectionReason,
                    submission.Documents.Select(d => new DocumentDto(
                        d.UserDocument.Id,
                        d.UserDocument.OriginalFileName,
                        d.UserDocument.FilePath
                    )).ToList()
                ) : null
            );
        }).ToList();

       var allSubmissions = application.Submissions
        .Where(s => s.Documents.Any())
        .OrderBy(s => service.Steps.FirstOrDefault(st => st.Id == s.StepId)?.Order ?? 0)
        .ThenBy(s => s.CreatedAt)
        .Select(sub => new SubmissionDto(
            sub.Id,
            sub.StepId,
            service.Steps.FirstOrDefault(st => st.Id == sub.StepId)?.Order ?? 0,
            service.Steps.FirstOrDefault(st => st.Id == sub.StepId)?.Name ?? "",
            sub.FormData,
            sub.Status.ToString(),
            sub.RejectionReason,
            sub.CreatedAt,
            sub.Documents.Select(d => new DocumentDto(
                d.UserDocument.Id,
                d.UserDocument.OriginalFileName,
                d.UserDocument.FilePath
            )).ToList()
        )).ToList();

        var dto = new ApplicationDetailsDto(
            application.Id,
            application.ServiceId,
            service.Name,
            service.Description,
            service.ServiceType.ToString(),
            application.ApplicantId,
            user.Username,
            application.Status.ToString(),
            application.CurrentStep,
            application.ScheduledDateTime,
            application.CompletedAt,
            application.AdminNotes,
            application.CreatedAt,
            stepDetails,
            allSubmissions,
            null
        );

        return Result<ApplicationDetailsDto>.Success(dto);
    }

    private static UploadConfigDto? ParseUploadConfig(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return null;

        try
        {
            return System.Text.Json.JsonSerializer.Deserialize<UploadConfigDto>(json);
        }
        catch
        {
            return null;
        }
    }
}

public record ApplicationDetailsDto(
    long Id,
    long ServiceId,
    string ServiceName,
    string ServiceDescription,
    string ServiceType,
    long ApplicantId,
    string ApplicantName,
    string Status,
    int CurrentStep,
    DateTime? ScheduledDateTime,
    DateTime? CompletedAt,
    string? AdminNotes,
    DateTime CreatedAt,
    List<StepDetailDto> Steps,
    List<SubmissionDto> Submissions,
    List<EquipmentDto>? RegisteredEquipment
);

public record StepDetailDto(
    long Id,
    string Name,
    int Order,
    string? Instructions,
    string? DownloadableFormUrl,
    bool RequiresFileUpload,
    bool RequiresTextInput,
    bool IsOptional,
    bool RequiresApproval,
    UploadConfigDto? UploadConfig,
    StepSubmissionDto? Submission
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

public record SubmissionDto(
    long Id,
    long StepId,
    int StepOrder,
    string StepName,
    string? FormData,
    string Status,
    string? RejectionReason,
    DateTime SubmittedAt,
    List<DocumentDto> Documents
);

public record DocumentDto(
    long Id,
    string FileName,
    string FilePath
);

public record EquipmentDto(
    string Name,
    string SerialNumber
);
