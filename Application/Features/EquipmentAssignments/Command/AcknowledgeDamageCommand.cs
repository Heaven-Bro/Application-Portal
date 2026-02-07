namespace Application.Features.EquipmentAssignments.Commands;

using MediatR;
using FluentValidation;
using Domain.Repositories;
using Domain.Common.Enums;
using Application.Common.Interfaces;
using Domain.Notifications;

public sealed record AcknowledgeDamageCommand(long AssignmentId) : IRequest<Unit>;

public sealed class AcknowledgeDamageCommandValidator : AbstractValidator<AcknowledgeDamageCommand>
{
    public AcknowledgeDamageCommandValidator()
    {
        RuleFor(x => x.AssignmentId)
            .GreaterThan(0).WithMessage("Assignment ID is required");
    }
}

public sealed class AcknowledgeDamageCommandHandler : IRequestHandler<AcknowledgeDamageCommand, Unit>
{
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly IEquipmentAssignmentRepository _assignmentRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationRepository _notificationRepository;

    public AcknowledgeDamageCommandHandler(
        IEquipmentRepository equipmentRepository,
        IEquipmentAssignmentRepository assignmentRepository,
        IUserRepository userRepository,
        ICurrentUserService currentUserService,
        INotificationRepository notificationRepository)
    {
        _equipmentRepository = equipmentRepository;
        _assignmentRepository = assignmentRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
        _notificationRepository = notificationRepository;
    }

    public async Task<Unit> Handle(AcknowledgeDamageCommand request, CancellationToken cancellationToken)
    {
        var assignment = await _assignmentRepository.GetByIdAsync(request.AssignmentId, cancellationToken);
        
        if (assignment == null)
            throw new Exception($"Assignment with ID {request.AssignmentId} not found");

        var equipment = await _equipmentRepository.GetByIdAsync(assignment.EquipmentId, cancellationToken);
        
        if (equipment == null)
            throw new Exception($"Equipment with ID {assignment.EquipmentId} not found");

        var currentUserId = _currentUserService.UserId;

        assignment.AcknowledgeDamage(currentUserId);
        equipment.UpdateConditionAndAvailability(EquipmentCondition.Damaged, false, currentUserId);

        _assignmentRepository.Update(assignment);
        _equipmentRepository.Update(equipment);
        
        await _assignmentRepository.SaveChangesAsync(cancellationToken);

        await NotifyAdmins(assignment.ApplicationId, equipment.Name, currentUserId, cancellationToken);

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

        var message = $"{applicantName} acknowledged damage for {equipmentName}.";

        var notifications = adminIds
            .Select(id => Notification.Create(
                id,
                NotificationType.DamageAcknowledged,
                applicationId,
                message,
                null,
                applicantId))
            .ToList();

        await _notificationRepository.AddRangeAsync(notifications, cancellationToken);
        await _notificationRepository.SaveChangesAsync(cancellationToken);
    }
}







