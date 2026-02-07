namespace Application.Features.EquipmentAssignments.Commands;

using MediatR;
using FluentValidation;
using Domain.Repositories;
using Domain.Common.Enums;
using Application.Common.Interfaces;
using Domain.Notifications;

public sealed record MarkAsDamagedCommand(
    long AssignmentId,
    string AdminNotes
) : IRequest<Unit>;

public sealed class MarkAsDamagedCommandValidator : AbstractValidator<MarkAsDamagedCommand>
{
    public MarkAsDamagedCommandValidator()
    {
        RuleFor(x => x.AssignmentId)
            .GreaterThan(0).WithMessage("Assignment ID is required");

        RuleFor(x => x.AdminNotes)
            .NotEmpty().WithMessage("Admin notes are required when marking equipment as damaged")
            .MaximumLength(1000).WithMessage("Admin notes must not exceed 1000 characters");
    }
}

public sealed class MarkAsDamagedCommandHandler : IRequestHandler<MarkAsDamagedCommand, Unit>
{
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly IEquipmentAssignmentRepository _assignmentRepository;
    private readonly IApplicationRepository _applicationRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationRepository _notificationRepository;

    public MarkAsDamagedCommandHandler(
        IEquipmentRepository equipmentRepository,
        IEquipmentAssignmentRepository assignmentRepository,
        IApplicationRepository applicationRepository,
        ICurrentUserService currentUserService,
        INotificationRepository notificationRepository)
    {
        _equipmentRepository = equipmentRepository;
        _assignmentRepository = assignmentRepository;
        _applicationRepository = applicationRepository;
        _currentUserService = currentUserService;
        _notificationRepository = notificationRepository;
    }

    public async Task<Unit> Handle(MarkAsDamagedCommand request, CancellationToken cancellationToken)
    {
        var assignment = await _assignmentRepository.GetByIdAsync(request.AssignmentId, cancellationToken);
        
        if (assignment == null)
            throw new Exception($"Assignment with ID {request.AssignmentId} not found");

        var equipment = await _equipmentRepository.GetByIdAsync(assignment.EquipmentId, cancellationToken);
        
        if (equipment == null)
            throw new Exception($"Equipment with ID {assignment.EquipmentId} not found");

        var application = await _applicationRepository.GetByIdAsync(assignment.ApplicationId, cancellationToken);
        if (application == null)
            throw new Exception($"Application with ID {assignment.ApplicationId} not found");

        var currentUserId = _currentUserService.UserId;
        assignment.MarkAsDamaged(request.AdminNotes, currentUserId);
        equipment.UpdateConditionAndAvailability(EquipmentCondition.Damaged, false, currentUserId);

        _assignmentRepository.Update(assignment);
        _equipmentRepository.Update(equipment);
        await _assignmentRepository.SaveChangesAsync(cancellationToken);

        var message = $"Return marked as damaged for {equipment.Name}. Notes: {request.AdminNotes}";
        var notification = Notification.Create(
            application.ApplicantId,
            NotificationType.EquipmentMarkedDamaged,
            application.Id,
            message,
            null,
            currentUserId);
        await _notificationRepository.AddAsync(notification, cancellationToken);
        await _notificationRepository.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}







