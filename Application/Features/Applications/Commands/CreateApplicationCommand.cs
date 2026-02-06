namespace Application.Features.Applications.Commands;

using MediatR;
using Application.Common.Interfaces;
using Application.Common.Models;
using Shared.Contracts.Services;
using Shared.Contracts.Common;
using Domain.Repositories;

public sealed record CreateApplicationCommand(long ServiceId) : IRequest<Result<long>>;

public sealed class CreateApplicationCommandHandler : IRequestHandler<CreateApplicationCommand, Result<long>>
{
    private readonly IApplicationRepository _applicationRepository;
    private readonly IServiceRepository _serviceRepository;
    private readonly ICurrentUserService _currentUserService;

    public CreateApplicationCommandHandler(
        IApplicationRepository applicationRepository,
        IServiceRepository serviceRepository,
        ICurrentUserService currentUserService)
    {
        _applicationRepository = applicationRepository;
        _serviceRepository = serviceRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<long>> Handle(CreateApplicationCommand request, CancellationToken cancellationToken)
    {
        var service = await _serviceRepository.GetByIdAsync(request.ServiceId, cancellationToken);
        
        if (service == null)
            return Result<long>.Failure("Service not found");

        if (!service.IsActive)
            return Result<long>.Failure("Service is not active");

        var currentUserId = _currentUserService.UserId;

        var application = Domain.Applications.Application.Create(
            request.ServiceId,
            currentUserId
        );

        await _applicationRepository.AddAsync(application, cancellationToken);
        await _applicationRepository.SaveChangesAsync(cancellationToken);

        return Result<long>.Success(application.Id);
    }
}







