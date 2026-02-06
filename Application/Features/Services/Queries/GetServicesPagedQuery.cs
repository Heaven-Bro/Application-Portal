namespace Application.Features.Services.Queries;

using Application.Common.Models;
using Domain.Repositories;
using MediatR;
using Shared.Contracts.Services;
using Shared.Contracts.Common;


public record GetServicesPagedQuery(
    int Page = 1,
    int PageSize = 20,
    string? SearchTerm = null,
    int? ServiceType = null
) : IRequest<Result<PagedResult<ServiceDto>>>;

public class GetServicesPagedQueryHandler : IRequestHandler<GetServicesPagedQuery, Result<PagedResult<ServiceDto>>>
{
    private readonly IServiceRepository _serviceRepository;

    public GetServicesPagedQueryHandler(IServiceRepository serviceRepository)
    {
        _serviceRepository = serviceRepository;
    }

    public async Task<Result<PagedResult<ServiceDto>>> Handle(GetServicesPagedQuery request, CancellationToken cancellationToken)
    {
        var allServices = await _serviceRepository.GetAllActiveAsync(cancellationToken);
        var query = allServices.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(s => 
                s.Name.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                s.Description.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase));
        }

        if (request.ServiceType.HasValue)
        {
            query = query.Where(s => (int)s.ServiceType == request.ServiceType.Value);
        }

        var totalCount = query.Count();
        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        var items = query
            .OrderByDescending(s => s.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(s => new ServiceDto(
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
            ))
            .ToList();

        return Result<PagedResult<ServiceDto>>.Success(new PagedResult<ServiceDto>
        {
            Items = items,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = totalPages
        });
    }

    private static UploadConfigDto? ParseUploadConfig(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return null;

        try
        {
            return System.Text.Json.JsonSerializer.Deserialize<UploadConfigDto>(json);
        }
        catch
        {
            return null;
        }
    }
}







