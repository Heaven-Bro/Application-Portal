namespace Application.Features.EquipmentAssignments.Commands;

using MediatR;
using FluentValidation;
using Domain.Repositories;
using Application.Common.Interfaces;
using Domain.Notifications;
using Domain.Common.Enums;

public sealed record RejectReturnCommand(
    long AssignmentId,
    string AdminNotes
) : IRequest<Unit>;

public sealed class RejectReturnCommandValidator : AbstractValidator<RejectReturnCommand>
{
    public RejectReturnCommandValidator()
    {
        RuleFor(x => x.AssignmentId)
            .GreaterThan(0).WithMessage("Assignment ID is required");

        RuleFor(x => x.AdminNotes)
            .NotEmpty().WithMessage("Admin notes are required when rejecting return")
            .MaximumLength(1000).WithMessage("Admin notes must not exceed 1000 characters");
    }
}

public sealed class RejectReturnCommandHandler : IRequestHandler<RejectReturnCommand, Unit>
{
    private readonly IEquipmentAssignmentRepository _assignmentRepository;
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly IApplicationRepository _applicationRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationRepository _notificationRepository;

    public RejectReturnCommandHandler(
        IEquipmentAssignmentRepository assignmentRepository,
        IEquipmentRepository equipmentRepository,
        IApplicationRepository applicationRepository,
        ICurrentUserService currentUserService,
        INotificationRepository notificationRepository)
    {
        _assignmentRepository = assignmentRepository;
        _equipmentRepository = equipmentRepository;
        _applicationRepository = applicationRepository;
        _currentUserService = currentUserService;
        _notificationRepository = notificationRepository;
    }

    public async Task<Unit> Handle(RejectReturnCommand request, CancellationToken cancellationToken)
    {
        var assignment = await _assignmentRepository.GetByIdAsync(request.AssignmentId, cancellationToken);
        
        if (assignment == null)
            throw new Exception($"Assignment with ID {request.AssignmentId} not found");

        var equipment = await _equipmentRepository.GetByIdAsync(assignment.EquipmentId, cancellationToken);
        var application = await _applicationRepository.GetByIdAsync(assignment.ApplicationId, cancellationToken);

        var currentUserId = _currentUserService.UserId;
        assignment.RejectReturn(request.AdminNotes, currentUserId);

        _assignmentRepository.Update(assignment);
        await _assignmentRepository.SaveChangesAsync(cancellationToken);

        if (equipment != null && application != null)
        {
            var message = $"Return request rejected for {equipment.Name}. Notes: {request.AdminNotes}";
            var notification = Notification.Create(
                application.ApplicantId,
                NotificationType.ReturnRejected,
                application.Id,
                message,
                null,
                currentUserId);
            await _notificationRepository.AddAsync(notification, cancellationToken);
            await _notificationRepository.SaveChangesAsync(cancellationToken);
        }

        return Unit.Value;
    }
}







