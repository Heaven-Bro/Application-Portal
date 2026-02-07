namespace Application.Features.Equipment.Queries;

using MediatR;
using Domain.Repositories;
using Domain.Common.Enums;

public sealed record GetEquipmentByIdQuery(long Id) : IRequest<EquipmentDetailDto?>;

public sealed class GetEquipmentByIdQueryHandler : IRequestHandler<GetEquipmentByIdQuery, EquipmentDetailDto?>
{
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly IEquipmentAssignmentRepository _assignmentRepository;

    public GetEquipmentByIdQueryHandler(
        IEquipmentRepository equipmentRepository,
        IEquipmentAssignmentRepository assignmentRepository)
    {
        _equipmentRepository = equipmentRepository;
        _assignmentRepository = assignmentRepository;
    }

    public async Task<EquipmentDetailDto?> Handle(GetEquipmentByIdQuery request, CancellationToken cancellationToken)
    {
        var equipment = await _equipmentRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (equipment == null)
            return null;

        var assignments = await _assignmentRepository.GetByEquipmentIdAsync(equipment.Id, cancellationToken);
        assignments = assignments
            .GroupBy(a => new { a.ApplicationId, a.AssignedAt })
            .Select(g => g.OrderByDescending(x => x.Id).First())
            .ToList();

        bool IsActiveStatus(EquipmentAssignmentStatus status) => status is
            EquipmentAssignmentStatus.CheckedOut or
            EquipmentAssignmentStatus.ReturnRequested or
            EquipmentAssignmentStatus.ReturnRejected or
            EquipmentAssignmentStatus.PendingDamageAcknowledgment or
            EquipmentAssignmentStatus.DamageDisputed;

        var activeAssignment = assignments.FirstOrDefault(a => IsActiveStatus(a.Status));

        var isOverdue = activeAssignment != null &&
                        activeAssignment.ExpectedReturnDate.HasValue &&
                        activeAssignment.ExpectedReturnDate.Value.Date < DateTime.UtcNow.Date &&
                        (activeAssignment.Status == EquipmentAssignmentStatus.CheckedOut ||
                         activeAssignment.Status == EquipmentAssignmentStatus.ReturnRequested ||
                         activeAssignment.Status == EquipmentAssignmentStatus.ReturnRejected);

        string currentStatus = "Available";
        if (activeAssignment != null)
        {
            currentStatus = activeAssignment.Status switch
            {
                EquipmentAssignmentStatus.CheckedOut => isOverdue ? "Overdue" : "In Use",
                EquipmentAssignmentStatus.ReturnRequested => "Return Pending",
                EquipmentAssignmentStatus.PendingDamageAcknowledgment => "Damaged (Pending)",
                EquipmentAssignmentStatus.DamageDisputed => "Damage Disputed",
                EquipmentAssignmentStatus.ReturnRejected => "Return Rejected",
                _ => "In Use"
            };
        }
        else if (assignments.Any(a => a.Status == EquipmentAssignmentStatus.ReturnedDamaged))
        {
            currentStatus = "Needs Repair";
        }

        bool isConditionBad = equipment.Condition != EquipmentCondition.Good &&
                              equipment.Condition != EquipmentCondition.Fair;
        bool actualAvailability = equipment.IsAvailable && !isConditionBad && activeAssignment == null;

        return new EquipmentDetailDto
        {
            Id = equipment.Id,
            Name = equipment.Name,
            EquipmentCode = equipment.EquipmentCode,
            Category = equipment.Category,
            Description = equipment.Description,
            IsAvailable = actualAvailability,
            Condition = (int)equipment.Condition,
            CurrentStatus = currentStatus,
            AssignmentHistory = assignments.Select(a => new AssignmentHistoryDto
            {
                Id = a.Id,
                ApplicationId = a.ApplicationId,
                AssignedAt = a.AssignedAt,
                ExpectedReturnDate = a.ExpectedReturnDate,
                ReturnRequestedAt = a.ReturnRequestedAt,
                ReturnVerifiedAt = a.ReturnVerifiedAt,
                Status = a.Status.ToString(),
                AdminNotes = a.AdminNotes,
                ApplicantResponse = a.ApplicantResponse,
                DamageAcknowledgedAt = a.DamageAcknowledgedAt,
                IsOverdue = a.ExpectedReturnDate.HasValue && 
                           a.ExpectedReturnDate.Value.Date < DateTime.UtcNow.Date &&
                           (a.Status == EquipmentAssignmentStatus.CheckedOut || 
                            a.Status == EquipmentAssignmentStatus.ReturnRequested ||
                            a.Status == EquipmentAssignmentStatus.ReturnRejected)
            }).OrderByDescending(a => a.AssignedAt).ToList()
        };
    }
}

public class EquipmentDetailDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string EquipmentCode { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsAvailable { get; set; }
    public int Condition { get; set; }
    public string CurrentStatus { get; set; } = string.Empty;
    public List<AssignmentHistoryDto> AssignmentHistory { get; set; } = new();
}

public class AssignmentHistoryDto
{
    public long Id { get; set; }
    public long ApplicationId { get; set; }
    public DateTime AssignedAt { get; set; }
    public DateTime? ExpectedReturnDate { get; set; }
    public DateTime? ReturnRequestedAt { get; set; }
    public DateTime? ReturnVerifiedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? AdminNotes { get; set; }
    public string? ApplicantResponse { get; set; }
    public DateTime? DamageAcknowledgedAt { get; set; }
    public bool IsOverdue { get; set; }
}







