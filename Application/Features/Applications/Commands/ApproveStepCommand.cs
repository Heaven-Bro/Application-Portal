namespace Application.Features.Applications.Commands;

using MediatR;
using Application.Common.Models;
using Shared.Contracts.Services;
using Shared.Contracts.Common;
using Application.Common.Interfaces;
using Domain.Repositories;

public record ApproveStepCommand(long SubmissionId) : IRequest<Result>;

public class ApproveStepCommandHandler(
    IApplicationRepository applicationRepository,
    IServiceRepository serviceRepository,
    ICurrentUserService currentUser) : IRequestHandler<ApproveStepCommand, Result>
{
    public async Task<Result> Handle(ApproveStepCommand request, CancellationToken cancellationToken)
    {
        var applications = await applicationRepository.GetPendingForAdminAsync(cancellationToken);
        var application = applications
            .FirstOrDefault(a => a.Submissions.Any(s => s.Id == request.SubmissionId));

        if (application == null)
            return Result.Failure("Submission not found");

        var submission = application.Submissions.FirstOrDefault(s => s.Id == request.SubmissionId);
        if (submission == null)
            return Result.Failure("Submission not found");

        var service = await serviceRepository.GetByIdWithStepsAsync(application.ServiceId, cancellationToken);
        if (service == null)
            return Result.Failure("Service not found");

        var step = service.Steps.FirstOrDefault(s => s.Id == submission.StepId);
        if (step == null)
            return Result.Failure("Step not found");

        application.ApproveStep(request.SubmissionId, currentUser.UserId, step.RequiresApproval);

        if (step.RequiresApproval)
        {
            application.AdvanceStep();
        }

        applicationRepository.Update(application);
        await applicationRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
