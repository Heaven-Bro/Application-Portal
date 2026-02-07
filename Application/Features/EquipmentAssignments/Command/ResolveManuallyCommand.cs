namespace Application.Features.EquipmentAssignments.Commands;

using MediatR;
using FluentValidation;
using Domain.Repositories;
using Domain.Common.Enums;
using Application.Common.Interfaces;
using Domain.Notifications;

public sealed record ResolveManuallyCommand(
    long AssignmentId,
    EquipmentAssignmentStatus FinalStatus,
    string Notes
) : IRequest<Unit>;

public sealed class ResolveManuallyCommandValidator : AbstractValidator<ResolveManuallyCommand>
{
    public ResolveManuallyCommandValidator()
    {
        RuleFor(x => x.AssignmentId)
            .GreaterThan(0).WithMessage("Assignment ID is required");

        RuleFor(x => x.FinalStatus)
            .IsInEnum().WithMessage("Valid final status is required")
            .Must(status => status == EquipmentAssignmentStatus.ReturnedGood || 
                           status == EquipmentAssignmentStatus.ReturnedDamaged)
            .WithMessage("Final status must be ReturnedGood or ReturnedDamaged");

        RuleFor(x => x.Notes)
            .NotEmpty().WithMessage("Notes are required for manual resolution")
            .MaximumLength(1000).WithMessage("Notes must not exceed 1000 characters");
    }
}

public sealed class ResolveManuallyCommandHandler : IRequestHandler<ResolveManuallyCommand, Unit>
{
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly IEquipmentAssignmentRepository _assignmentRepository;
    private readonly IApplicationRepository _applicationRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationRepository _notificationRepository;

    public ResolveManuallyCommandHandler(
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

    public async Task<Unit> Handle(ResolveManuallyCommand request, CancellationToken cancellationToken)
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

        assignment.ResolveManually(request.FinalStatus, request.Notes, currentUserId);
        if (request.FinalStatus == EquipmentAssignmentStatus.ReturnedGood)
        {
            equipment.MarkAsAvailable(currentUserId);
        }
        else
        {
            equipment.UpdateConditionAndAvailability(EquipmentCondition.Damaged, false, currentUserId);
        }

        _assignmentRepository.Update(assignment);
        _equipmentRepository.Update(equipment);
        
        await _assignmentRepository.SaveChangesAsync(cancellationToken);

        var isGood = request.FinalStatus == EquipmentAssignmentStatus.ReturnedGood;
        var type = isGood ? NotificationType.ReturnApproved : NotificationType.EquipmentMarkedDamaged;
        var message = isGood
            ? $"Your return was approved for {equipment.Name}. Notes: {request.Notes}"
            : $"Return resolved as damaged for {equipment.Name}. Notes: {request.Notes}";

        var notification = Notification.Create(
            application.ApplicantId,
            type,
            application.Id,
            message,
            null,
            currentUserId);
        await _notificationRepository.AddAsync(notification, cancellationToken);
        await _notificationRepository.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}







