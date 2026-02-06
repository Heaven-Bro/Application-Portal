namespace Application.Features.Applications.Queries;

using MediatR;
using Application.Common;
using Domain.Repositories;

public record GetAdminApplicationDetailsQuery(long ApplicationId) : IRequest<Result<AdminApplicationDetailsDto>>;

public class GetAdminApplicationDetailsQueryHandler : IRequestHandler<GetAdminApplicationDetailsQuery, Result<AdminApplicationDetailsDto>>
{
    private readonly IApplicationRepository _applicationRepository;
    private readonly IServiceRepository _serviceRepository;
    private readonly IUserRepository _userRepository;
    private readonly IEquipmentAssignmentRepository _equipmentAssignmentRepository;
    private readonly IEquipmentRepository _equipmentRepository;

    public GetAdminApplicationDetailsQueryHandler(
        IApplicationRepository applicationRepository,
        IServiceRepository serviceRepository,
        IUserRepository userRepository,
        IEquipmentAssignmentRepository equipmentAssignmentRepository,
        IEquipmentRepository equipmentRepository)
    {
        _applicationRepository = applicationRepository;
        _serviceRepository = serviceRepository;
        _userRepository = userRepository;
        _equipmentAssignmentRepository = equipmentAssignmentRepository;
        _equipmentRepository = equipmentRepository;
    }

    public async Task<Result<AdminApplicationDetailsDto>> Handle(GetAdminApplicationDetailsQuery request, CancellationToken cancellationToken)
    {
        var application = await _applicationRepository.GetByIdWithSubmissionsAsync(request.ApplicationId, cancellationToken);
        
        if (application == null)
            return Result<AdminApplicationDetailsDto>.Failure("Application not found");

        var service = await _serviceRepository.GetByIdWithStepsAsync(application.ServiceId, cancellationToken);
        if (service == null)
            return Result<AdminApplicationDetailsDto>.Failure("Service not found");

        var user = await _userRepository.GetByIdAsync(application.ApplicantId, cancellationToken);
        if (user == null)
            return Result<AdminApplicationDetailsDto>.Failure("User not found");

        var stepDetails = service.Steps.OrderBy(s => s.Order).Select(step =>
        {
            var submissions = application.Submissions
                .Where(s => s.StepId == step.Id)
                .OrderByDescending(s => s.CreatedAt)
                .Select(sub => new AdminStepSubmissionDto(
                    sub.Id,
                    sub.StepId,
                    step.Order,
                    step.Name,
                    sub.FormData,
                    sub.Status.ToString(),
                    sub.CreatedAt,
                    sub.ReviewedAt,
                    sub.RejectionReason,
                    sub.IsLatest,
                    sub.Documents.Select(d => new AdminDocumentDto(
                        d.UserDocument.Id,
                        d.UserDocument.OriginalFileName,
                        d.UserDocument.FilePath
                    )).ToList()
                )).ToList();

            return new AdminStepDetailDto(
                step.Id,
                step.Name,
                step.Order,
                step.Instructions,
                step.RequiresFileUpload,
                step.RequiresTextInput,
                step.IsOptional,
                step.RequiresApproval,
                submissions
            );
        }).ToList();

        var assignments = await _equipmentAssignmentRepository.GetByApplicationIdAsync(application.Id, cancellationToken);
        
        var assignmentDtos = new List<EquipmentAssignmentSummaryDto>();
        foreach (var assignment in assignments)
        {
            var equipment = await _equipmentRepository.GetByIdAsync(assignment.EquipmentId, cancellationToken);
            if (equipment != null)
            {
                assignmentDtos.Add(new EquipmentAssignmentSummaryDto(
                    assignment.Id,
                    assignment.EquipmentId,
                    equipment.Name,
                    equipment.EquipmentCode,
                    assignment.AssignedAt,
                    assignment.ExpectedReturnDate,
                    assignment.Status.ToString()
                ));
            }
        }
        
        var dto = new AdminApplicationDetailsDto(
            application.Id,
            application.ServiceId,
            service.Name,
            service.Description,
            service.ServiceType.ToString(),
            application.ApplicantId,
            user.Username,
            user.Email,
            application.Status.ToString(),
            application.CurrentStep,
            service.Steps.Count,
            application.ScheduledDateTime,
            application.CompletedAt,
            application.AdminNotes,
            application.CreatedAt,
            stepDetails,
            assignmentDtos
        );

        return Result<AdminApplicationDetailsDto>.Success(dto);
    }
}

public record AdminApplicationDetailsDto(
    long Id,
    long ServiceId,
    string ServiceName,
    string ServiceDescription,
    string ServiceType,
    long ApplicantId,
    string ApplicantName,
    string ApplicantEmail,
    string Status,
    int CurrentStep,
    int TotalSteps,
    DateTime? ScheduledDateTime,
    DateTime? CompletedAt,
    string? AdminNotes,
    DateTime CreatedAt,
    List<AdminStepDetailDto> Steps,
    List<EquipmentAssignmentSummaryDto> EquipmentAssignments
);

public record AdminStepDetailDto(
    long Id,
    string Name,
    int Order,
    string? Instructions,
    bool RequiresFileUpload,
    bool RequiresTextInput,
    bool IsOptional,
    bool RequiresApproval,
    List<AdminStepSubmissionDto> Submissions
);

public record AdminStepSubmissionDto(
    long Id,
    long StepId,
    int StepOrder,
    string StepName,
    string? FormData,
    string Status,
    DateTime SubmittedAt,
    DateTime? ReviewedAt,
    string? RejectionReason,
    bool IsLatest,
    List<AdminDocumentDto> Documents
);

public record AdminDocumentDto(
    long Id,
    string FileName,
    string FilePath
);

public record EquipmentAssignmentSummaryDto(
    long Id,
    long EquipmentId,
    string EquipmentName,
    string EquipmentCode,
    DateTime AssignedAt,
    DateTime? ExpectedReturnDate,
    string Status
);
