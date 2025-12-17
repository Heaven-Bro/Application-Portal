namespace Application.Features.Services.Commands;

using MediatR;
using Application.Common.Models;
using Application.Common.Interfaces;
using Domain.Repositories;
using Domain.Services;

public record CreateServiceCommand(
    string Name,
    string Description,
    List<CreateServiceStepDto> Steps
) : IRequest<Result<long>>;

public record CreateServiceStepDto(
    string Name,
    string Description,
    int Order,
    bool RequiresFileUpload,
    bool RequiresTextInput,
    bool IsEquipmentAssignment
);

public class CreateServiceCommandHandler(
    IServiceRepository serviceRepository,
    ICurrentUserService currentUser) : IRequestHandler<CreateServiceCommand, Result<long>>
{
    public async Task<Result<long>> Handle(CreateServiceCommand request, CancellationToken cancellationToken)
    {
        var service = Service.Create(request.Name, request.Description, currentUser.UserId);

        foreach (var step in request.Steps.OrderBy(s => s.Order))
        {
            service.AddStep(
                step.Name,
                step.Description,
                step.Order,
                step.RequiresFileUpload,
                step.RequiresTextInput,
                step.IsEquipmentAssignment
            );
        }

        await serviceRepository.AddAsync(service, cancellationToken);
        await serviceRepository.SaveChangesAsync(cancellationToken);

        return Result<long>.Success(service.Id);
    }
}
