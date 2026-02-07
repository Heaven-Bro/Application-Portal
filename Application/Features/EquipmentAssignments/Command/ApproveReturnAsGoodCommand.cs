namespace Application.Features.EquipmentAssignments.Commands;

using MediatR;
using FluentValidation;
using Domain.Repositories;
using Application.Common.Interfaces;
using Domain.Notifications;
using Domain.Common.Enums;

public sealed record ApproveReturnAsGoodCommand(long AssignmentId) : IRequest<Unit>;

public sealed class ApproveReturnAsGoodCommandValidator : AbstractValidator<ApproveReturnAsGoodCommand>
{
    public ApproveReturnAsGoodCommandValidator()
    {
        RuleFor(x => x.AssignmentId)
            .GreaterThan(0).WithMessage("Assignment ID is required");
    }
}

public sealed class ApproveReturnAsGoodCommandHandler : IRequestHandler<ApproveReturnAsGoodCommand, Unit>
{
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly IEquipmentAssignmentRepository _assignmentRepository;
    private readonly IApplicationRepository _applicationRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationRepository _notificationRepository;

    public ApproveReturnAsGoodCommandHandler(
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

    public async Task<Unit> Handle(ApproveReturnAsGoodCommand request, CancellationToken cancellationToken)
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

        assignment.ApproveReturnAsGood(currentUserId);
        equipment.MarkAsAvailable(currentUserId);

        _assignmentRepository.Update(assignment);
        _equipmentRepository.Update(equipment);
        
        await _assignmentRepository.SaveChangesAsync(cancellationToken);

        var message = $"Your return request for {equipment.Name} was approved.";
        var notification = Notification.Create(
            application.ApplicantId,
            NotificationType.ReturnApproved,
            assignment.ApplicationId,
            message,
            null,
            currentUserId);
        await _notificationRepository.AddAsync(notification, cancellationToken);
        await _notificationRepository.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}







