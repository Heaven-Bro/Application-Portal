namespace Application.Features.Applications.Commands;

using MediatR;
using Application.Common.Models;
using Application.Common.Interfaces;
using Domain.Repositories;

public record ApproveStepCommand(long SubmissionId) : IRequest<Result>;

public class ApproveStepCommandHandler(
    IApplicationRepository applicationRepository,
    ICurrentUserService currentUser) : IRequestHandler<ApproveStepCommand, Result>
{
    public async Task<Result> Handle(ApproveStepCommand request, CancellationToken cancellationToken)
    {
        var applications = await applicationRepository.GetPendingForAdminAsync(cancellationToken);
        var application = applications
            .FirstOrDefault(a => a.Submissions.Any(s => s.Id == request.SubmissionId));

        if (application == null)
            return Result.Failure("Submission not found");

        application.ApproveStep(request.SubmissionId, currentUser.UserId);

        applicationRepository.Update(application);
        await applicationRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
