namespace Application.Features.Applications.Commands;

using MediatR;
using Application.Common;
using Domain.Repositories;
using Application.Common.Interfaces;

public record ScheduleAppointmentCommand(
    long ApplicationId,
    DateTime ScheduledDateTime,
    string? Notes
) : IRequest<Result>;

public class ScheduleAppointmentCommandHandler : IRequestHandler<ScheduleAppointmentCommand, Result>
{
    private readonly IApplicationRepository _applicationRepository;
    private readonly ICurrentUserService _currentUserService;

    public ScheduleAppointmentCommandHandler(
        IApplicationRepository applicationRepository,
        ICurrentUserService currentUserService)
    {
        _applicationRepository = applicationRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(ScheduleAppointmentCommand request, CancellationToken cancellationToken)
    {
        var application = await _applicationRepository.GetByIdAsync(request.ApplicationId, cancellationToken);
        
        if (application == null)
            return Result.Failure("Application not found");

        application.MarkAsApproved(request.ScheduledDateTime, _currentUserService.UserId, request.Notes);

        _applicationRepository.Update(application);
        await _applicationRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
