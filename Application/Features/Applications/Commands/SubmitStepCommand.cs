namespace Application.Features.Applications.Commands;

using MediatR;
using Application.Common.Models;
using Application.Common.Interfaces;
using Domain.Repositories;

public record SubmitStepCommand(
    long ApplicationId,
    long StepId,
    string? FormData,
    List<string> FilePaths
) : IRequest<Result<long>>;

public class SubmitStepCommandHandler(
    IApplicationRepository applicationRepository,
    ICurrentUserService currentUser) : IRequestHandler<SubmitStepCommand, Result<long>>
{
    public async Task<Result<long>> Handle(SubmitStepCommand request, CancellationToken cancellationToken)
    {
        var application = await applicationRepository.GetByIdWithSubmissionsAsync(
            request.ApplicationId, cancellationToken);

        if (application == null)
            return Result<long>.Failure("Application not found");

        if (application.ApplicantId != currentUser.UserId)
            return Result<long>.Failure("Unauthorized");

        var submission = application.SubmitStep(request.StepId, request.FormData, request.FilePaths);

        applicationRepository.Update(application);
        await applicationRepository.SaveChangesAsync(cancellationToken);

        return Result<long>.Success(submission.Id);
    }
}
