namespace Application.Features.Equipment.Commands;

using MediatR;
using FluentValidation;
using Domain.Repositories;
using Domain.Common.Enums;
using Application.Common.Interfaces;

public sealed record UpdateEquipmentConditionCommand(
    long EquipmentId,
    EquipmentCondition Condition
) : IRequest<Unit>;

public sealed class UpdateEquipmentConditionCommandValidator : AbstractValidator<UpdateEquipmentConditionCommand>
{
    public UpdateEquipmentConditionCommandValidator()
    {
        RuleFor(x => x.EquipmentId)
            .GreaterThan(0).WithMessage("Equipment ID is required");

        RuleFor(x => x.Condition)
            .IsInEnum().WithMessage("Valid equipment condition is required");
    }
}

public sealed class UpdateEquipmentConditionCommandHandler : IRequestHandler<UpdateEquipmentConditionCommand, Unit>
{
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly IEquipmentAssignmentRepository _assignmentRepository;
    private readonly ICurrentUserService _currentUserService;

    public UpdateEquipmentConditionCommandHandler(
        IEquipmentRepository equipmentRepository,
        IEquipmentAssignmentRepository assignmentRepository,
        ICurrentUserService currentUserService)
    {
        _equipmentRepository = equipmentRepository;
        _assignmentRepository = assignmentRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(UpdateEquipmentConditionCommand request, CancellationToken cancellationToken)
    {
        var equipment = await _equipmentRepository.GetByIdAsync(request.EquipmentId, cancellationToken);

        if (equipment == null)
            throw new Exception($"Equipment with ID {request.EquipmentId} not found");

        bool IsLockedStatus(EquipmentAssignmentStatus status) => status is
            EquipmentAssignmentStatus.CheckedOut or
            EquipmentAssignmentStatus.ReturnRequested or
            EquipmentAssignmentStatus.ReturnRejected;

        var assignments = await _assignmentRepository.GetByEquipmentIdAsync(request.EquipmentId, cancellationToken);
        if (assignments.Any(a => IsLockedStatus(a.Status)))
            throw new Exception("Cannot change condition while equipment is checked out or return is pending");

        var currentUserId = _currentUserService.UserId;

        var isAvailable = request.Condition is EquipmentCondition.Good or EquipmentCondition.Fair;
        equipment.UpdateConditionAndAvailability(request.Condition, isAvailable, currentUserId);

        _equipmentRepository.Update(equipment);
        await _equipmentRepository.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
