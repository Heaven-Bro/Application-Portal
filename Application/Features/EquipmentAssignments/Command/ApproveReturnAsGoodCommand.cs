namespace Application.Features.EquipmentAssignments.Commands;

using MediatR;
using FluentValidation;
using Domain.Repositories;
using Application.Common.Interfaces;

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
    private readonly ICurrentUserService _currentUserService;

    public ApproveReturnAsGoodCommandHandler(
        IEquipmentRepository equipmentRepository,
        IEquipmentAssignmentRepository assignmentRepository,
        ICurrentUserService currentUserService)
    {
        _equipmentRepository = equipmentRepository;
        _assignmentRepository = assignmentRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(ApproveReturnAsGoodCommand request, CancellationToken cancellationToken)
    {
        var assignment = await _assignmentRepository.GetByIdAsync(request.AssignmentId, cancellationToken);
        
        if (assignment == null)
            throw new Exception($"Assignment with ID {request.AssignmentId} not found");

        var equipment = await _equipmentRepository.GetByIdAsync(assignment.EquipmentId, cancellationToken);
        
        if (equipment == null)
            throw new Exception($"Equipment with ID {assignment.EquipmentId} not found");

        var currentUserId = _currentUserService.UserId;

        assignment.ApproveReturnAsGood(currentUserId);
        equipment.MarkAsAvailable(currentUserId);

        _assignmentRepository.Update(assignment);
        _equipmentRepository.Update(equipment);
        
        await _assignmentRepository.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}







