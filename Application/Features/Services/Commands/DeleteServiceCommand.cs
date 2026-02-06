namespace Application.Features.Services.Commands;

using Application.Common.Models;
using Shared.Contracts.Services;
using Shared.Contracts.Common;
using Domain.Repositories;
using MediatR;

public record DeleteServiceCommand(long ServiceId, long DeletedBy) : IRequest<Result>;

public class DeleteServiceCommandHandler : IRequestHandler<DeleteServiceCommand, Result>
{
    private readonly IServiceRepository _serviceRepository;
    private readonly IApplicationRepository _applicationRepository;

    public DeleteServiceCommandHandler(
        IServiceRepository serviceRepository,
        IApplicationRepository applicationRepository)
    {
        _serviceRepository = serviceRepository;
        _applicationRepository = applicationRepository;
    }

    public async Task<Result> Handle(DeleteServiceCommand request, CancellationToken cancellationToken)
    {
        var service = await _serviceRepository.GetByIdAsync(request.ServiceId, cancellationToken);
        if (service == null)
            return Result.Failure("Service not found");

        var hasPendingApplications = await _applicationRepository
            .HasPendingApplicationsByServiceIdAsync(request.ServiceId, cancellationToken);

        if (hasPendingApplications)
            return Result.Failure("Cannot delete service with pending applications");

        await _serviceRepository.DeleteAsync(service, cancellationToken);

        return Result.Success();
    }
}







