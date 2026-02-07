namespace Application.Features.Applications.Commands;

using MediatR;
using Application.Common.Interfaces;
using Application.Common.Models;
using Shared.Contracts.Services;
using Shared.Contracts.Common;
using Domain.Repositories;
using Domain.Notifications;
using Domain.Common.Enums;

public sealed record CreateApplicationCommand(long ServiceId) : IRequest<Result<long>>;

public sealed class CreateApplicationCommandHandler : IRequestHandler<CreateApplicationCommand, Result<long>>
{
    private readonly IApplicationRepository _applicationRepository;
    private readonly IServiceRepository _serviceRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserRepository _userRepository;
    private readonly INotificationRepository _notificationRepository;

    public CreateApplicationCommandHandler(
        IApplicationRepository applicationRepository,
        IServiceRepository serviceRepository,
        ICurrentUserService currentUserService,
        IUserRepository userRepository,
        INotificationRepository notificationRepository)
    {
        _applicationRepository = applicationRepository;
        _serviceRepository = serviceRepository;
        _currentUserService = currentUserService;
        _userRepository = userRepository;
        _notificationRepository = notificationRepository;
    }

    public async Task<Result<long>> Handle(CreateApplicationCommand request, CancellationToken cancellationToken)
    {
        var service = await _serviceRepository.GetByIdAsync(request.ServiceId, cancellationToken);
        
        if (service == null)
            return Result<long>.Failure("Service not found");

        if (!service.IsActive)
            return Result<long>.Failure("Service is not active");

        var currentUserId = _currentUserService.UserId;

        var application = Domain.Applications.Application.Create(
            request.ServiceId,
            currentUserId
        );

        await _applicationRepository.AddAsync(application, cancellationToken);
        await _applicationRepository.SaveChangesAsync(cancellationToken);

        await NotifyAdmins(service.Name, currentUserId, application.Id, cancellationToken);

        return Result<long>.Success(application.Id);
    }

    private async Task NotifyAdmins(string serviceName, long applicantId, long applicationId, CancellationToken cancellationToken)
    {
        var applicant = await _userRepository.GetByIdAsync(applicantId, cancellationToken);
        var applicantName = applicant?.Name ?? applicant?.Username ?? "Applicant";

        var admins = await _userRepository.GetAllAsync(cancellationToken);
        var adminIds = admins
            .Where(u => u.Role == UserRole.Admin && u.Status == UserStatus.Active)
            .Select(u => u.Id)
            .ToList();

        if (!adminIds.Any())
            return;

        var message = $"New application submitted by {applicantName} for {serviceName}.";

        var notifications = adminIds
            .Select(id => Notification.Create(
                id,
                NotificationType.NewApplication,
                applicationId,
                message,
                null,
                applicantId))
            .ToList();

        await _notificationRepository.AddRangeAsync(notifications, cancellationToken);
        await _notificationRepository.SaveChangesAsync(cancellationToken);
    }
}







