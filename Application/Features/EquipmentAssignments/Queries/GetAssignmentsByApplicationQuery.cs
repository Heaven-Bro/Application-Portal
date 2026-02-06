namespace Application.Features.EquipmentAssignments.Queries;

using MediatR;
using Domain.Repositories;
using Domain.Common.Enums;

public sealed record GetAssignmentsByApplicationQuery(long ApplicationId) : IRequest<List<EquipmentAssignmentDto>>;

public sealed class GetAssignmentsByApplicationQueryHandler : IRequestHandler<GetAssignmentsByApplicationQuery, List<EquipmentAssignmentDto>>
{
    private readonly IEquipmentAssignmentRepository _assignmentRepository;
    private readonly IEquipmentRepository _equipmentRepository;

    public GetAssignmentsByApplicationQueryHandler(
        IEquipmentAssignmentRepository assignmentRepository,
        IEquipmentRepository equipmentRepository)
    {
        _assignmentRepository = assignmentRepository;
        _equipmentRepository = equipmentRepository;
    }

    public async Task<List<EquipmentAssignmentDto>> Handle(GetAssignmentsByApplicationQuery request, CancellationToken cancellationToken)
    {
        var assignments = await _assignmentRepository.GetByApplicationIdAsync(request.ApplicationId, cancellationToken);
        var result = new List<EquipmentAssignmentDto>();

        foreach (var assignment in assignments)
        {
            var equipment = await _equipmentRepository.GetByIdAsync(assignment.EquipmentId, cancellationToken);
            
            result.Add(new EquipmentAssignmentDto
            {
                Id = assignment.Id,
                ApplicationId = assignment.ApplicationId,
                EquipmentId = assignment.EquipmentId,
                EquipmentName = equipment?.Name ?? "Unknown",
                EquipmentCode = equipment?.EquipmentCode ?? "Unknown",
                AssignedAt = assignment.AssignedAt,
                ExpectedReturnDate = assignment.ExpectedReturnDate,
                ReturnRequestedAt = assignment.ReturnRequestedAt,
                ReturnVerifiedAt = assignment.ReturnVerifiedAt,
                Status = assignment.Status,
                AdminNotes = assignment.AdminNotes,
                ApplicantResponse = assignment.ApplicantResponse
            });
        }

        return result;
    }
}

public class EquipmentAssignmentDto
{
    public long Id { get; set; }
    public long ApplicationId { get; set; }
    public long EquipmentId { get; set; }
    public string EquipmentName { get; set; } = string.Empty;
    public string EquipmentCode { get; set; } = string.Empty;
    public DateTime AssignedAt { get; set; }
    public DateTime? ExpectedReturnDate { get; set; }
    public DateTime? ReturnRequestedAt { get; set; }
    public DateTime? ReturnVerifiedAt { get; set; }
    public EquipmentAssignmentStatus Status { get; set; }
    public string? AdminNotes { get; set; }
    public string? ApplicantResponse { get; set; }
}







