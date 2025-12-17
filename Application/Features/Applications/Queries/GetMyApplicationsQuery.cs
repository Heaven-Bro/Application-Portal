namespace Application.Features.Applications.Queries;

using MediatR;
using Application.Common.Models;
using Application.Common.Interfaces;
using Domain.Repositories;

public record GetMyApplicationsQuery : IRequest<Result<List<ApplicationDto>>>;

public class GetMyApplicationsQueryHandler(
    IApplicationRepository applicationRepository,
    IServiceRepository serviceRepository,
    IUserRepository userRepository,
    ICurrentUserService currentUser) : IRequestHandler<GetMyApplicationsQuery, Result<List<ApplicationDto>>>
{
    public async Task<Result<List<ApplicationDto>>> Handle(GetMyApplicationsQuery request, CancellationToken cancellationToken)
    {
        var applications = await applicationRepository.GetByApplicantIdAsync(currentUser.UserId, cancellationToken);
        var dtos = new List<ApplicationDto>();

        foreach (var app in applications)
        {
            var service = await serviceRepository.GetByIdWithStepsAsync(app.ServiceId, cancellationToken);
            var user = await userRepository.GetByIdAsync(app.ApplicantId, cancellationToken);

            if (service == null || user == null) continue;

            var appWithSubmissions = await applicationRepository.GetByIdWithSubmissionsAsync(app.Id, cancellationToken);

            var submissionDtos = appWithSubmissions!.Submissions.Select(s =>
            {
                var step = service.Steps.FirstOrDefault(st => st.Id == s.StepId);
                return new StepSubmissionDto(
                    s.Id,
                    s.StepId,
                    step?.Name ?? "Unknown",
                    s.SubmissionVersion,
                    s.FormData,
                    s.GetFilePaths(),
                    s.Status.ToString(),
                    s.CreatedAt,
                    s.RejectionReason
                );
            }).ToList();

            dtos.Add(new ApplicationDto(
                app.Id,
                app.ServiceId,
                service.Name,
                app.ApplicantId,
                user.Username,
                app.Status.ToString(),
                app.CurrentStep,
                app.ScheduledDateTime,
                app.CreatedAt,
                submissionDtos
            ));
        }

        return Result<List<ApplicationDto>>.Success(dtos);
    }
}
