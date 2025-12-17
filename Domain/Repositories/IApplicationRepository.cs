namespace Domain.Repositories;

using Domain.Applications;
using Domain.Common.Enums;

public interface IApplicationRepository
{
    Task<Application?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<Application?> GetByIdWithSubmissionsAsync(long id, CancellationToken cancellationToken = default);
    Task<List<Application>> GetByApplicantIdAsync(long applicantId, CancellationToken cancellationToken = default);
    Task<List<Application>> GetByStatusAsync(ApplicationStatus status, CancellationToken cancellationToken = default);
    Task<List<Application>> GetPendingForAdminAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Application application, CancellationToken cancellationToken = default);
    void Update(Application application);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
