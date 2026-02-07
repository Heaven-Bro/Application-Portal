namespace Application.Features.Applications.Commands;

using MediatR;
using Application.Common.Models;
using Shared.Contracts.Services;
using Shared.Contracts.Common;
using Application.Common.Interfaces;
using Domain.Repositories;
using Domain.Notifications;
using Domain.Common.Enums;

public record RejectStepCommand(long SubmissionId, string Reason) : IRequest<Result>;

public class RejectStepCommandHandler(
    IApplicationRepository applicationRepository,
    ICurrentUserService currentUser,
    INotificationRepository notificationRepository) : IRequestHandler<RejectStepCommand, Result>
{
    public async Task<Result> Handle(RejectStepCommand request, CancellationToken cancellationToken)
    {
        var applications = await applicationRepository.GetPendingForAdminAsync(cancellationToken);
        var application = applications
            .FirstOrDefault(a => a.Submissions.Any(s => s.Id == request.SubmissionId));

        if (application == null)
            return Result.Failure("Submission not found");

        application.RejectStep(request.SubmissionId, request.Reason, currentUser.UserId);

        applicationRepository.Update(application);
        await applicationRepository.SaveChangesAsync(cancellationToken);

        var message = $"Your application was rejected. Reason: {request.Reason}";
        var notification = Notification.Create(
            application.ApplicantId,
            NotificationType.ApplicationRejected,
            application.Id,
            message,
            null,
            currentUser.UserId);
        await notificationRepository.AddAsync(notification, cancellationToken);
        await notificationRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
