namespace Application.Features.Equipment.Queries;

using MediatR;
using Domain.Repositories;

public record SearchAvailableEquipmentQuery(string SearchTerm) : IRequest<List<AvailableEquipmentDto>>;

public class SearchAvailableEquipmentQueryHandler : IRequestHandler<SearchAvailableEquipmentQuery, List<AvailableEquipmentDto>>
{
    private readonly IEquipmentRepository _equipmentRepository;

    public SearchAvailableEquipmentQueryHandler(IEquipmentRepository equipmentRepository)
    {
        _equipmentRepository = equipmentRepository;
    }

    public async Task<List<AvailableEquipmentDto>> Handle(SearchAvailableEquipmentQuery request, CancellationToken cancellationToken)
    {
        var equipments = await _equipmentRepository.GetAllAsync(cancellationToken);
        
        var availableEquipments = equipments
            .Where(e => e.IsAvailable && 
                       (e.Condition == Domain.Common.Enums.EquipmentCondition.Good || 
                        e.Condition == Domain.Common.Enums.EquipmentCondition.Fair));

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var search = request.SearchTerm.ToLower();
            availableEquipments = availableEquipments.Where(e => 
                e.Name.ToLower().Contains(search) || 
                e.EquipmentCode.ToLower().Contains(search) ||
                e.Category.ToLower().Contains(search));
        }

        return availableEquipments
            .Select(e => new AvailableEquipmentDto(
                e.Id,
                e.Name,
                e.EquipmentCode,
                e.Category,
                e.Condition.ToString(),
                e.Description
            ))
            .OrderBy(e => e.EquipmentCode)
            .ToList();
    }
}

public record AvailableEquipmentDto(
    long Id,
    string Name,
    string EquipmentCode,
    string Category,
    string Condition,
    string? Description
);
