namespace Application.Features.Applications.Queries;

using MediatR;
using Domain.Repositories;
using Domain.Common.Enums;

public record GetAdminApplicationsQuery(
    int Page = 1,
    int PageSize = 20,
    string? Search = null,
    string? Status = null,
    string? ServiceType = null,
    long? ApplicantId = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null,
    bool? ReturnRequestsOnly = null
) : IRequest<PaginatedResult<AdminApplicationListItemDto>>;

public class GetAdminApplicationsQueryHandler : IRequestHandler<GetAdminApplicationsQuery, PaginatedResult<AdminApplicationListItemDto>>
{
    private readonly IApplicationRepository _applicationRepository;
    private readonly IServiceRepository _serviceRepository;
    private readonly IUserRepository _userRepository;
    private readonly IEquipmentAssignmentRepository _equipmentAssignmentRepository;

    public GetAdminApplicationsQueryHandler(
        IApplicationRepository applicationRepository,
        IServiceRepository serviceRepository,
        IUserRepository userRepository,
        IEquipmentAssignmentRepository equipmentAssignmentRepository)
    {
        _applicationRepository = applicationRepository;
        _serviceRepository = serviceRepository;
        _userRepository = userRepository;
        _equipmentAssignmentRepository = equipmentAssignmentRepository;
    }

    public async Task<PaginatedResult<AdminApplicationListItemDto>> Handle(GetAdminApplicationsQuery request, CancellationToken cancellationToken)
    {
        var query = await _applicationRepository.GetAllForAdminAsync(cancellationToken);
        
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchLower = request.Search.ToLower();
            var users = await _userRepository.SearchUsersAsync(searchLower, cancellationToken);
            var userIds = users.Select(u => u.Id).ToList();
            
            query = query.Where(a => userIds.Contains(a.ApplicantId)).ToList();
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            if (Enum.TryParse<Domain.Common.Enums.ApplicationStatus>(request.Status, true, out var status))
            {
                query = query.Where(a => a.Status == status).ToList();
            }
        }

        if (request.ApplicantId.HasValue)
        {
            query = query.Where(a => a.ApplicantId == request.ApplicantId.Value).ToList();
        }

        if (request.FromDate.HasValue)
        {
            query = query.Where(a => a.CreatedAt >= request.FromDate.Value).ToList();
        }

        if (request.ToDate.HasValue)
        {
            query = query.Where(a => a.CreatedAt <= request.ToDate.Value.AddDays(1)).ToList();
        }

        if (!string.IsNullOrWhiteSpace(request.ServiceType))
        {
            var services = await _serviceRepository.GetAllAsync(cancellationToken);
            if (Enum.TryParse<Domain.Common.Enums.ServiceType>(request.ServiceType, true, out var serviceType))
            {
                var serviceIds = services.Where(s => s.ServiceType == serviceType).Select(s => s.Id).ToList();
                query = query.Where(a => serviceIds.Contains(a.ServiceId)).ToList();
            }
        }

        bool IsPendingReturn(EquipmentAssignmentStatus status) => status is
            EquipmentAssignmentStatus.ReturnRequested or
            EquipmentAssignmentStatus.PendingDamageAcknowledgment or
            EquipmentAssignmentStatus.DamageDisputed;

        if (request.ReturnRequestsOnly == true)
        {
            var appIds = query.Select(a => a.Id).ToList();
            var assignments = await _equipmentAssignmentRepository.GetByApplicationIdsAsync(appIds, cancellationToken);
            var pendingAppIds = assignments
                .Where(a => IsPendingReturn(a.Status))
                .Select(a => a.ApplicationId)
                .Distinct()
                .ToHashSet();

            query = query.Where(a => pendingAppIds.Contains(a.Id)).ToList();
        }

        query = query.OrderByDescending(a => a.CreatedAt).ToList();

        var totalCount = query.Count;
        var items = query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var itemIds = items.Select(a => a.Id).ToList();
        var itemAssignments = await _equipmentAssignmentRepository.GetByApplicationIdsAsync(itemIds, cancellationToken);
        var pendingCountByAppId = itemAssignments
            .Where(a => IsPendingReturn(a.Status))
            .GroupBy(a => a.ApplicationId)
            .ToDictionary(g => g.Key, g => g.Count());

        var dtos = new List<AdminApplicationListItemDto>();

        foreach (var app in items)
        {
            var service = await _serviceRepository.GetByIdAsync(app.ServiceId, cancellationToken);
            var user = await _userRepository.GetByIdAsync(app.ApplicantId, cancellationToken);

            if (service != null && user != null)
            {
                pendingCountByAppId.TryGetValue(app.Id, out var pendingCount);
                dtos.Add(new AdminApplicationListItemDto(
                    app.Id,
                    app.ServiceId,
                    service.Name,
                    service.ServiceType.ToString(),
                    app.ApplicantId,
                    user.Username,
                    user.Email,
                    app.Status.ToString(),
                    app.CurrentStep,
                    app.ScheduledDateTime,
                    app.CreatedAt,
                    pendingCount
                ));
            }
        }

        return new PaginatedResult<AdminApplicationListItemDto>(
            dtos,
            totalCount,
            request.Page,
            request.PageSize
        );
    }
}

public record AdminApplicationListItemDto(
    long Id,
    long ServiceId,
    string ServiceName,
    string ServiceType,
    long ApplicantId,
    string ApplicantName,
    string ApplicantEmail,
    string Status,
    int CurrentStep,
    DateTime? ScheduledDateTime,
    DateTime CreatedAt,
    int PendingReturnCount
);

public record PaginatedResult<T>(
    List<T> Items,
    int TotalCount,
    int Page,
    int PageSize
)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;
}
