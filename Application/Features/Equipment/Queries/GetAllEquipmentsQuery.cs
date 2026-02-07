namespace Application.Features.Equipment.Queries;

using MediatR;
using Domain.Repositories;
using Shared.Contracts.Common;
using Domain.Common.Enums;

public sealed record GetAllEquipmentsQuery(
    int Page = 1,
    int PageSize = 10,
    string? Search = null,
    bool? IsAvailable = null,
    bool? IsOverdue = null
) : IRequest<PagedResult<EquipmentDto>>;


public sealed class GetAllEquipmentsQueryHandler : IRequestHandler<GetAllEquipmentsQuery, PagedResult<EquipmentDto>>
{
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly IEquipmentAssignmentRepository _assignmentRepository;

    public GetAllEquipmentsQueryHandler(
        IEquipmentRepository equipmentRepository,
        IEquipmentAssignmentRepository assignmentRepository)
    {
        _equipmentRepository = equipmentRepository;
        _assignmentRepository = assignmentRepository;
    }

    public async Task<PagedResult<EquipmentDto>> Handle(GetAllEquipmentsQuery request, CancellationToken cancellationToken)
    {
        var allEquipment = await _equipmentRepository.GetAllAsync(cancellationToken);
        var equipmentWithAssignments = new List<EquipmentDto>();

        foreach (var equipment in allEquipment)
        {
            var assignments = await _assignmentRepository.GetByEquipmentIdAsync(equipment.Id, cancellationToken);
            
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

            bool isConditionBad = equipment.Condition != EquipmentCondition.Good && 
                                  equipment.Condition != EquipmentCondition.Fair;

            string currentStatus;
            
            if (isConditionBad)
            {
                currentStatus = "Unavailable";
            }
            else if (activeAssignment != null)
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
            else
            {
                currentStatus = equipment.IsAvailable ? "Ready" : "Unavailable";
            }

            bool actualAvailability = equipment.IsAvailable && !isConditionBad && activeAssignment == null;

            equipmentWithAssignments.Add(new EquipmentDto
            {
                Id = equipment.Id,
                Name = equipment.Name,
                EquipmentCode = equipment.EquipmentCode,
                Category = equipment.Category,
                Description = equipment.Description,
                IsAvailable = actualAvailability,
                Condition = equipment.Condition.ToString(),
                CurrentStatus = currentStatus,
                CurrentApplicationId = activeAssignment?.ApplicationId,
                IsOverdue = isOverdue,
                ExpectedReturnDate = activeAssignment?.ExpectedReturnDate
            });
        }

        var filtered = equipmentWithAssignments.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.ToLower();
            filtered = filtered.Where(e => 
                e.Name.ToLower().Contains(search) || 
                e.EquipmentCode.ToLower().Contains(search) ||
                e.Category.ToLower().Contains(search));
        }

        if (request.IsAvailable.HasValue)
        {
            filtered = filtered.Where(e => e.IsAvailable == request.IsAvailable.Value);
        }

        if (request.IsOverdue == true)
        {
            filtered = filtered.Where(e => e.IsOverdue);
        }

        var totalCount = filtered.Count();
        var items = filtered
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        return new PagedResult<EquipmentDto>
        {
            Items = items,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
        };
    }
}

public class EquipmentDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string EquipmentCode { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsAvailable { get; set; }
    public string Condition { get; set; } = string.Empty;
    public string CurrentStatus { get; set; } = string.Empty;
    public long? CurrentApplicationId { get; set; }
    public bool IsOverdue { get; set; }
    public DateTime? ExpectedReturnDate { get; set; }
}







