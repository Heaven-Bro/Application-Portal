namespace Application.Features.EquipmentAssignments.Queries;

using MediatR;
using Domain.Repositories;
using Domain.Common.Enums;

public sealed record GetOverdueAssignmentsQuery : IRequest<List<OverdueAssignmentDto>>;

public sealed class GetOverdueAssignmentsQueryHandler : IRequestHandler<GetOverdueAssignmentsQuery, List<OverdueAssignmentDto>>
{
    private readonly IEquipmentAssignmentRepository _assignmentRepository;
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly IApplicationRepository _applicationRepository;

    public GetOverdueAssignmentsQueryHandler(
        IEquipmentAssignmentRepository assignmentRepository,
        IEquipmentRepository equipmentRepository,
        IApplicationRepository applicationRepository)
    {
        _assignmentRepository = assignmentRepository;
        _equipmentRepository = equipmentRepository;
        _applicationRepository = applicationRepository;
    }

    public async Task<List<OverdueAssignmentDto>> Handle(GetOverdueAssignmentsQuery request, CancellationToken cancellationToken)
    {
        var overdueAssignments = await _assignmentRepository.GetOverdueAsync(cancellationToken);
        var result = new List<OverdueAssignmentDto>();

        foreach (var assignment in overdueAssignments)
        {
            var equipment = await _equipmentRepository.GetByIdAsync(assignment.EquipmentId, cancellationToken);
            var application = await _applicationRepository.GetByIdAsync(assignment.ApplicationId, cancellationToken);
            
            result.Add(new OverdueAssignmentDto
            {
                AssignmentId = assignment.Id,
                ApplicationId = assignment.ApplicationId,
                ApplicantId = application?.ApplicantId ?? 0,
                EquipmentName = equipment?.Name ?? "Unknown",
                EquipmentCode = equipment?.EquipmentCode ?? "Unknown",
                AssignedAt = assignment.AssignedAt,
                ExpectedReturnDate = assignment.ExpectedReturnDate,
                DaysOverdue = assignment.ExpectedReturnDate.HasValue 
                    ? (DateTime.UtcNow.Date - assignment.ExpectedReturnDate.Value.Date).Days 
                    : 0,
                Status = assignment.Status
            });
        }

        return result.OrderByDescending(x => x.DaysOverdue).ToList();
    }
}

public class OverdueAssignmentDto
{
    public long AssignmentId { get; set; }
    public long ApplicationId { get; set; }
    public long ApplicantId { get; set; }
    public string EquipmentName { get; set; } = string.Empty;
    public string EquipmentCode { get; set; } = string.Empty;
    public DateTime AssignedAt { get; set; }
    public DateTime? ExpectedReturnDate { get; set; }
    public int DaysOverdue { get; set; }
    public EquipmentAssignmentStatus Status { get; set; }
}







