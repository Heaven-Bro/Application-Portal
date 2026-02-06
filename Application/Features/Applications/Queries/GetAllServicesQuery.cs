namespace Application.Features.Applications.Queries;

using Application.Common.Models;
using Shared.Contracts.Services;
using Shared.Contracts.Common;
using Domain.Repositories;
using MediatR;

public record GetAllServicesQuery : IRequest<Result<List<ServiceDto>>>;

public class GetAllServicesQueryHandler : IRequestHandler<GetAllServicesQuery, Result<List<ServiceDto>>>
{
    private readonly IServiceRepository _serviceRepository;

    public GetAllServicesQueryHandler(IServiceRepository serviceRepository)
    {
        _serviceRepository = serviceRepository;
    }

    public async Task<Result<List<ServiceDto>>> Handle(GetAllServicesQuery request, CancellationToken cancellationToken)
    {
        var services = await _serviceRepository.GetAllActiveAsync(cancellationToken);

        var serviceDtos = services.Select(s => new ServiceDto(
            s.Id,
            s.Name,
            s.Description,
            s.IsActive,
            s.ServiceType.ToString(),
            s.Steps.OrderBy(st => st.Order).Select(st => new ServiceStepDto(
                st.Id,
                st.Name,
                st.Order,
                st.RequiresFileUpload,
                st.RequiresTextInput,
                st.Instructions,
                st.DownloadableFormUrl,
                ParseUploadConfig(st.UploadConfig)
            )).ToList()
        )).ToList();

        return Result<List<ServiceDto>>.Success(serviceDtos);
    }

    private static UploadConfigDto? ParseUploadConfig(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return null;

        try
        {
            var config = System.Text.Json.JsonSerializer.Deserialize<UploadConfigDto>(json);
            return config;
        }
        catch
        {
            return null;
        }
    }
}







