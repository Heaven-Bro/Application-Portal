namespace Application.Features.Services.Commands;

using Application.Common.Models;
using Shared.Contracts.Services;
using Shared.Contracts.Common;
using Domain.Repositories;
using MediatR;

public record UpdateServiceCommand(
    long ServiceId,
    string Name,
    string Description,
    int ServiceType,
    bool IsActive,
    List<UpdateServiceStepDto> Steps
) : IRequest<Result>;

public record UpdateServiceStepDto(
    long? Id,
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

public class UpdateServiceCommandHandler : IRequestHandler<UpdateServiceCommand, Result>
{
    private readonly IServiceRepository _serviceRepository;

    public UpdateServiceCommandHandler(IServiceRepository serviceRepository)
    {
        _serviceRepository = serviceRepository;
    }

    public async Task<Result> Handle(UpdateServiceCommand request, CancellationToken cancellationToken)
    {
        var service = await _serviceRepository.GetByIdWithStepsAsync(request.ServiceId, cancellationToken);

        if (service == null)
            return Result.Failure("Service not found");

        service.UpdateDetails(request.Name, request.Description);
        
        if (request.IsActive)
            service.Activate();
        else
            service.Deactivate();

        var existingStepIds = service.Steps.Select(s => s.Id).ToList();
        var newStepIds = request.Steps.Where(s => s.Id.HasValue).Select(s => s.Id!.Value).ToList();
        var stepsToRemove = existingStepIds.Except(newStepIds).ToList();

        foreach (var stepId in stepsToRemove)
        {
            var stepToRemove = service.Steps.FirstOrDefault(s => s.Id == stepId);
            if (stepToRemove != null)
            {
                service.RemoveStep(stepToRemove);
            }
        }

        foreach (var stepDto in request.Steps)
        {
            if (stepDto.Id.HasValue)
            {
                var existingStep = service.Steps.FirstOrDefault(s => s.Id == stepDto.Id.Value);
                if (existingStep != null)
                {
                    existingStep.UpdateDetails(
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
            }
            else
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
        }

        await _serviceRepository.UpdateAsync(service, cancellationToken);

        return Result.Success();
    }
}
