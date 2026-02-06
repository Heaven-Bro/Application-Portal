namespace Application.Features.Equipment.Queries;

using MediatR;
using Domain.Repositories;

public record GetEquipmentDetailsQuery(long EquipmentId) : IRequest<EquipmentDetailsDto?>;

public class GetEquipmentDetailsQueryHandler : IRequestHandler<GetEquipmentDetailsQuery, EquipmentDetailsDto?>
{
    private readonly IEquipmentRepository _equipmentRepository;

    public GetEquipmentDetailsQueryHandler(IEquipmentRepository equipmentRepository)
    {
        _equipmentRepository = equipmentRepository;
    }

    public async Task<EquipmentDetailsDto?> Handle(GetEquipmentDetailsQuery request, CancellationToken cancellationToken)
    {
        var equipment = await _equipmentRepository.GetByIdAsync(request.EquipmentId, cancellationToken);
        
        if (equipment == null)
            return null;

        return new EquipmentDetailsDto(
            equipment.Id,
            equipment.Name,
            equipment.EquipmentCode,
            equipment.Category,
            equipment.Condition.ToString(),
            equipment.Description
        );
    }
}

public record EquipmentDetailsDto(
    long Id,
    string Name,
    string EquipmentCode,
    string Category,
    string Condition,
    string? Description
);
