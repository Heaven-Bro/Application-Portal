namespace Application.Features.Services.Commands;

using MediatR;
using Application.Common.Models;
using Shared.Contracts.Services;
using Shared.Contracts.Common;
using Application.Common.Interfaces;
using Domain.Repositories;

public record UpdateServiceDescriptionCommand(long ServiceId, string Description) : IRequest<Result>;

public class UpdateServiceDescriptionCommandHandler(
    IServiceRepository serviceRepository,
    ICurrentUserService currentUser) : IRequestHandler<UpdateServiceDescriptionCommand, Result>
{
    public async Task<Result> Handle(UpdateServiceDescriptionCommand request, CancellationToken cancellationToken)
    {
        var service = await serviceRepository.GetByIdAsync(request.ServiceId, cancellationToken);
        if (service == null)
            return Result.Failure("Service not found");

        service.UpdateDescription(request.Description, currentUser.UserId);

        serviceRepository.Update(service);
        await serviceRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}







