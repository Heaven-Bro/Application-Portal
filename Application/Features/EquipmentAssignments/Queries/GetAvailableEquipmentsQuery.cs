namespace Application.Features.EquipmentAssignments.Queries;

using MediatR;
using Domain.Repositories;

public sealed record GetAvailableEquipmentsQuery : IRequest<List<AvailableEquipmentDto>>;

public sealed class GetAvailableEquipmentsQueryHandler : IRequestHandler<GetAvailableEquipmentsQuery, List<AvailableEquipmentDto>>
{
    private readonly IEquipmentRepository _equipmentRepository;

    public GetAvailableEquipmentsQueryHandler(IEquipmentRepository equipmentRepository)
    {
        _equipmentRepository = equipmentRepository;
    }

    public async Task<List<AvailableEquipmentDto>> Handle(GetAvailableEquipmentsQuery request, CancellationToken cancellationToken)
    {
        var equipment = await _equipmentRepository.GetAvailableAsync(cancellationToken);

        return equipment.Select(e => new AvailableEquipmentDto
        {
            Id = e.Id,
            Name = e.Name,
            EquipmentCode = e.EquipmentCode,
            Category = e.Category
        }).ToList();
    }
}

public class AvailableEquipmentDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string EquipmentCode { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
}







