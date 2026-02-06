namespace Application.Features.Services.Queries;

using MediatR;
using Application.Common.Models;
using Shared.Contracts.Services;
using Shared.Contracts.Common;
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
            s.ServiceType.ToString(),
            s.Steps.OrderBy(st => st.Order).Select(step => new ServiceStepDto(
                step.Id,
                step.Name,
                step.Order,
                step.RequiresFileUpload,
                step.RequiresTextInput,
                step.Instructions,
                step.DownloadableFormUrl,
                ParseUploadConfig(step.UploadConfig)
            )).ToList()
        )).ToList();

        return Result<List<ServiceDto>>.Success(dtos);
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







