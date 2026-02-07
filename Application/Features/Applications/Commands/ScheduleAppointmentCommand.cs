namespace Application.Features.Applications.Commands;

using MediatR;
using Application.Common;
using Domain.Repositories;
using Application.Common.Interfaces;
using Domain.Notifications;
using Domain.Common.Enums;

public record ScheduleAppointmentCommand(
    long ApplicationId,
    DateTime ScheduledDateTime,
    string? Notes
) : IRequest<Result>;

public class ScheduleAppointmentCommandHandler : IRequestHandler<ScheduleAppointmentCommand, Result>
{
    private readonly IApplicationRepository _applicationRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationRepository _notificationRepository;

    public ScheduleAppointmentCommandHandler(
        IApplicationRepository applicationRepository,
        ICurrentUserService currentUserService,
        INotificationRepository notificationRepository)
    {
        _applicationRepository = applicationRepository;
        _currentUserService = currentUserService;
        _notificationRepository = notificationRepository;
    }

    public async Task<Result> Handle(ScheduleAppointmentCommand request, CancellationToken cancellationToken)
    {
        var application = await _applicationRepository.GetByIdAsync(request.ApplicationId, cancellationToken);
        
        if (application == null)
            return Result.Failure("Application not found");

        application.MarkAsApproved(request.ScheduledDateTime, _currentUserService.UserId, request.Notes);

        _applicationRepository.Update(application);
        await _applicationRepository.SaveChangesAsync(cancellationToken);

        var message = $"Your application has been approved. Appointment scheduled for {request.ScheduledDateTime:MMM dd, yyyy 'at' hh:mm tt}.";
        var notification = Notification.Create(
            application.ApplicantId,
            NotificationType.AppointmentScheduled,
            application.Id,
            message,
            null,
            _currentUserService.UserId);
        await _notificationRepository.AddAsync(notification, cancellationToken);
        await _notificationRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
