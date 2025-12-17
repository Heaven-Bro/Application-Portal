namespace Application.Features.Services.Queries;

using Application.Common.Models;
using Domain.Repositories;
using MediatR;

public record GetAllServicesQuery : IRequest<Application.Common.Models.Result<List<ServiceDto>>>;

public class GetAllServicesQueryHandler : IRequestHandler<GetAllServicesQuery, Application.Common.Models.Result<List<ServiceDto>>>
{
    private readonly IServiceRepository _serviceRepository;

    public GetAllServicesQueryHandler(IServiceRepository serviceRepository)
    {
        _serviceRepository = serviceRepository;
    }

    public async Task<Application.Common.Models.Result<List<ServiceDto>>> Handle(GetAllServicesQuery request, CancellationToken cancellationToken)
    {
        var services = await _serviceRepository.GetAllActiveAsync(cancellationToken);

        var serviceDtos = services.Select(s => new ServiceDto(
            s.Id,
            s.Name,
            s.Description,
            s.IsActive,
            s.ServiceVersion,
            s.Steps.OrderBy(st => st.Order).Select(st => new ServiceStepDto(
                st.Id,
                st.Name,
                st.Description,
                st.Order,
                st.RequiresFileUpload,
                st.RequiresTextInput,
                st.IsEquipmentAssignment
            )).ToList()
        )).ToList();

				return Application.Common.Models.Result<List<ServiceDto>>.Success(serviceDtos);
    }
}
