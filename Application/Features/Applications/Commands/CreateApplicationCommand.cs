namespace Application.Features.Applications.Commands;

using MediatR;
using Application.Common.Models;
using Application.Common.Interfaces;
using Domain.Repositories;
using DomainApp = Domain.Applications.Application;

public record CreateApplicationCommand(long ServiceId) : IRequest<Result<long>>;

public class CreateApplicationCommandHandler(
    IApplicationRepository applicationRepository,
    IServiceRepository serviceRepository,
    ICurrentUserService currentUser) : IRequestHandler<CreateApplicationCommand, Result<long>>
{
    public async Task<Result<long>> Handle(CreateApplicationCommand request, CancellationToken cancellationToken)
    {
        var service = await serviceRepository.GetByIdAsync(request.ServiceId, cancellationToken);
        if (service == null)
            return Result<long>.Failure("Service not found");

        if (!service.IsActive)
            return Result<long>.Failure("Service is not active");

        var application = DomainApp.Create(
            service.Id,
            service.ServiceVersion,
            currentUser.UserId
        );

        await applicationRepository.AddAsync(application, cancellationToken);
        await applicationRepository.SaveChangesAsync(cancellationToken);

        return Result<long>.Success(application.Id);
    }
}
