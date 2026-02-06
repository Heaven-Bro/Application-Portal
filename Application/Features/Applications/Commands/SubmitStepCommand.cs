namespace Application.Features.Applications.Commands;

using MediatR;
using Application.Common.Interfaces;
using Application.Common.Models;
using Shared.Contracts.Services;
using Shared.Contracts.Common;
using Domain.Repositories;
using FluentValidation;

public sealed record SubmitStepCommand(
    long ApplicationId,
    long StepId,
    string? FormData,
    List<long> DocumentIds
) : IRequest<Result<long>>;

public sealed class SubmitStepCommandValidator : AbstractValidator<SubmitStepCommand>
{
    public SubmitStepCommandValidator()
    {
        RuleFor(x => x.ApplicationId)
            .GreaterThan(0).WithMessage("Application ID is required");

        RuleFor(x => x.StepId)
            .GreaterThan(0).WithMessage("Step ID is required");
    }
}

public sealed class SubmitStepCommandHandler : IRequestHandler<SubmitStepCommand, Result<long>>
{
    private readonly IApplicationRepository _applicationRepository;
    private readonly IServiceRepository _serviceRepository;
    private readonly IUserDocumentRepository _documentRepository;
    private readonly ICurrentUserService _currentUserService;

    public SubmitStepCommandHandler(
        IApplicationRepository applicationRepository,
        IServiceRepository serviceRepository,
        IUserDocumentRepository documentRepository,
        ICurrentUserService currentUserService)
    {
        _applicationRepository = applicationRepository;
        _serviceRepository = serviceRepository;
        _documentRepository = documentRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<long>> Handle(SubmitStepCommand request, CancellationToken cancellationToken)
    {
        var application = await _applicationRepository.GetByIdWithSubmissionsAsync(request.ApplicationId, cancellationToken);
        
        if (application == null)
            return Result<long>.Failure("Application not found");

        var currentUserId = _currentUserService.UserId;
        
        if (application.ApplicantId != currentUserId)
            return Result<long>.Failure("Unauthorized");

        var service = await _serviceRepository.GetByIdWithStepsAsync(application.ServiceId, cancellationToken);
        
        if (service == null)
            return Result<long>.Failure("Service not found");

        var step = service.Steps.FirstOrDefault(s => s.Id == request.StepId);
        
        if (step == null)
            return Result<long>.Failure("Step not found");

        if (step.RequiresTextInput && !step.IsOptional && string.IsNullOrWhiteSpace(request.FormData))
            return Result<long>.Failure("Text input is required for this step");

        if (step.RequiresFileUpload && !step.IsOptional && !request.DocumentIds.Any())
            return Result<long>.Failure("File upload is required for this step");

        if (request.DocumentIds.Any())
        {
            var documents = await _documentRepository.GetByIdsAsync(request.DocumentIds, cancellationToken);
            
            if (documents.Count != request.DocumentIds.Count)
                return Result<long>.Failure("Some documents not found");

            foreach (var doc in documents)
            {
                if (doc.UserId != currentUserId)
                    return Result<long>.Failure("You can only use your own documents");
            }
        }

        var submission = application.SubmitStepWithDocuments(
            request.StepId,
            request.FormData,
            request.DocumentIds
        );

        if (!step.RequiresApproval)
        {
            submission.Approve(currentUserId);
            
            var totalSteps = service.Steps.Count;
            if (step.Order < totalSteps)
            {
                application.AdvanceStep();
            }
        }

        _applicationRepository.Update(application);
        await _applicationRepository.SaveChangesAsync(cancellationToken);

        return Result<long>.Success(submission.Id);
    }
}
