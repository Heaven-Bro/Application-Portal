namespace Application.Features.EquipmentAssignments.Commands;

using MediatR;
using FluentValidation;
using Domain.Repositories;
using Domain.Common.Enums;
using Application.Common.Interfaces;

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
    private readonly ICurrentUserService _currentUserService;

    public ResolveManuallyCommandHandler(
        IEquipmentRepository equipmentRepository,
        IEquipmentAssignmentRepository assignmentRepository,
        ICurrentUserService currentUserService)
    {
        _equipmentRepository = equipmentRepository;
        _assignmentRepository = assignmentRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(ResolveManuallyCommand request, CancellationToken cancellationToken)
    {
        var assignment = await _assignmentRepository.GetByIdAsync(request.AssignmentId, cancellationToken);
        
        if (assignment == null)
            throw new Exception($"Assignment with ID {request.AssignmentId} not found");

        var equipment = await _equipmentRepository.GetByIdAsync(assignment.EquipmentId, cancellationToken);
        
        if (equipment == null)
            throw new Exception($"Equipment with ID {assignment.EquipmentId} not found");

        var currentUserId = _currentUserService.UserId;

        assignment.ResolveManually(request.FinalStatus, request.Notes, currentUserId);
        equipment.MarkAsAvailable(currentUserId);

        _assignmentRepository.Update(assignment);
        _equipmentRepository.Update(equipment);
        
        await _assignmentRepository.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}







