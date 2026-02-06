namespace Application.Features.Services.Commands;

using Application.Common.Models;
using Shared.Contracts.Services;
using Shared.Contracts.Common;
using Domain.Common.Enums;
using Domain.Repositories;
using Domain.Services;
using MediatR;

public record CreateServiceCommand(
    string Name,
    string Description,
    ServiceType ServiceType,
    List<CreateServiceStepDto> Steps,
    long CreatedBy
) : IRequest<Result<long>>;

public record CreateServiceStepDto(
    string Name,
    int Order,
    bool RequiresFileUpload,
    bool RequiresTextInput,
    string? Instructions,
    string? DownloadableFormUrl,
    bool IsOptional,
    string? UploadConfig,
     bool RequiresApproval
);

public class CreateServiceCommandHandler : IRequestHandler<CreateServiceCommand, Result<long>>
{
    private readonly IServiceRepository _serviceRepository;

    public CreateServiceCommandHandler(IServiceRepository serviceRepository)
    {
        _serviceRepository = serviceRepository;
    }


    public async Task<Result<long>> Handle(CreateServiceCommand request, CancellationToken cancellationToken)
    {
        var service = Service.Create(
            request.Name,
            request.Description,
            request.ServiceType,
            request.CreatedBy
        );

        foreach (var stepDto in request.Steps)
        {
            service.AddStep(
                stepDto.Name,
                stepDto.Order,
                stepDto.RequiresFileUpload,
                stepDto.RequiresTextInput,
                stepDto.Instructions,
                stepDto.DownloadableFormUrl,
                stepDto.IsOptional,
                stepDto.UploadConfig,
                stepDto.RequiresApproval
            );
        }

        await _serviceRepository.AddAsync(service, cancellationToken);
        await _serviceRepository.SaveChangesAsync(cancellationToken);

        return Result<long>.Success(service.Id);
    }

}







