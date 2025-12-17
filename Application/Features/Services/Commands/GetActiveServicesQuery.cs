namespace Application.Features.Services.Queries;

using MediatR;
using Application.Common.Models;
using Domain.Repositories;

public record GetActiveServicesQuery : IRequest<Result<List<ServiceDto>>>;

public class GetActiveServicesQueryHandler(
    IServiceRepository serviceRepository) : IRequestHandler<GetActiveServicesQuery, Result<List<ServiceDto>>>
{
    public async Task<Result<List<ServiceDto>>> Handle(GetActiveServicesQuery request, CancellationToken cancellationToken)
    {
        var services = await serviceRepository.GetAllActiveAsync(cancellationToken);

        var dtos = services.Select(s => new ServiceDto(
            s.Id,
            s.Name,
            s.Description,
            s.IsActive,
            s.ServiceVersion,
            s.Steps.Select(step => new ServiceStepDto(
                step.Id,
                step.Name,
                step.Description,
                step.Order,
                step.RequiresFileUpload,
                step.RequiresTextInput,
                step.IsEquipmentAssignment
            )).ToList()
        )).ToList();

        return Result<List<ServiceDto>>.Success(dtos);
    }
}
