namespace Application.Features.EquipmentAssignments.Commands;

using MediatR;
using FluentValidation;
using Domain.Repositories;
using Application.Common.Interfaces;
using Domain.Notifications;
using Domain.Common.Enums;

public sealed record DisputeDamageCommand(
    long AssignmentId,
    string ApplicantResponse
) : IRequest<Unit>;

public sealed class DisputeDamageCommandValidator : AbstractValidator<DisputeDamageCommand>
{
    public DisputeDamageCommandValidator()
    {
        RuleFor(x => x.AssignmentId)
            .GreaterThan(0).WithMessage("Assignment ID is required");

        RuleFor(x => x.ApplicantResponse)
            .NotEmpty().WithMessage("Applicant response is required when disputing damage")
            .MaximumLength(1000).WithMessage("Response must not exceed 1000 characters");
    }
}

public sealed class DisputeDamageCommandHandler : IRequestHandler<DisputeDamageCommand, Unit>
{
    private readonly IEquipmentAssignmentRepository _assignmentRepository;
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly IApplicationRepository _applicationRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationRepository _notificationRepository;

    public DisputeDamageCommandHandler(
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

    public async Task<Unit> Handle(DisputeDamageCommand request, CancellationToken cancellationToken)
    {
        var assignment = await _assignmentRepository.GetByIdAsync(request.AssignmentId, cancellationToken);
        
        if (assignment == null)
            throw new Exception($"Assignment with ID {request.AssignmentId} not found");

        var equipment = await _equipmentRepository.GetByIdAsync(assignment.EquipmentId, cancellationToken);
        var application = await _applicationRepository.GetByIdAsync(assignment.ApplicationId, cancellationToken);

        var currentUserId = _currentUserService.UserId;
        assignment.DisputeDamage(request.ApplicantResponse, currentUserId);

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

        var message = $"{applicantName} disputed damage for {equipmentName}.";

        var notifications = adminIds
            .Select(id => Notification.Create(
                id,
                NotificationType.DamageDisputed,
                applicationId,
                message,
                null,
                applicantId))
            .ToList();

        await _notificationRepository.AddRangeAsync(notifications, cancellationToken);
        await _notificationRepository.SaveChangesAsync(cancellationToken);
    }
}







