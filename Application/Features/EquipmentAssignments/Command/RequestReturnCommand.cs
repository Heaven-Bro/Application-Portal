namespace Application.Features.EquipmentAssignments.Commands;

using MediatR;
using FluentValidation;
using Domain.Repositories;
using Domain.Common.Enums;
using Application.Common.Interfaces;
using Domain.Notifications;

public sealed record RequestReturnCommand(long AssignmentId) : IRequest<Unit>;

public sealed class RequestReturnCommandValidator : AbstractValidator<RequestReturnCommand>
{
    public RequestReturnCommandValidator()
    {
        RuleFor(x => x.AssignmentId)
            .GreaterThan(0).WithMessage("Assignment ID is required");
    }
}

public sealed class RequestReturnCommandHandler : IRequestHandler<RequestReturnCommand, Unit>
{
    private readonly IEquipmentAssignmentRepository _assignmentRepository;
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly IApplicationRepository _applicationRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationRepository _notificationRepository;

    public RequestReturnCommandHandler(
        IEquipmentAssignmentRepository assignmentRepository,
        IEquipmentRepository equipmentRepository,
        IApplicationRepository applicationRepository,
        IUserRepository userRepository,
        ICurrentUserService currentUserService,
        INotificationRepository notificationRepository)
    {
        _assignmentRepository = assignmentRepository;
        _equipmentRepository = equipmentRepository;
        _applicationRepository = applicationRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
        _notificationRepository = notificationRepository;
    }

    public async Task<Unit> Handle(RequestReturnCommand request, CancellationToken cancellationToken)
    {
        var assignment = await _assignmentRepository.GetByIdAsync(request.AssignmentId, cancellationToken);
        
        if (assignment == null)
            throw new Exception($"Assignment with ID {request.AssignmentId} not found");

        var equipment = await _equipmentRepository.GetByIdAsync(assignment.EquipmentId, cancellationToken);
        var application = await _applicationRepository.GetByIdAsync(assignment.ApplicationId, cancellationToken);

        var currentUserId = _currentUserService.UserId;
        assignment.RequestReturn(currentUserId);

        _assignmentRepository.Update(assignment);
        await _assignmentRepository.SaveChangesAsync(cancellationToken);

        if (equipment != null && application != null)
        {
            await NotifyAdmins(application.Id, equipment.Name, currentUserId, cancellationToken);
        }

        return Unit.Value;
    }

    private async Task NotifyAdmins(long applicationId, string equipmentName, long applicantId, CancellationToken cancellationToken)
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

        var message = $"{applicantName} requested return for {equipmentName}.";

        var notifications = adminIds
            .Select(id => Notification.Create(
                id,
                NotificationType.ReturnRequested,
                applicationId,
                message,
                null,
                applicantId))
            .ToList();

        await _notificationRepository.AddRangeAsync(notifications, cancellationToken);
        await _notificationRepository.SaveChangesAsync(cancellationToken);
    }
}







