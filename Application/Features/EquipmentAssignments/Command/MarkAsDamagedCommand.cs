namespace Application.Features.EquipmentAssignments.Commands;

using MediatR;
using FluentValidation;
using Domain.Repositories;
using Application.Common.Interfaces;

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
    private readonly IEquipmentAssignmentRepository _assignmentRepository;
    private readonly ICurrentUserService _currentUserService;

    public MarkAsDamagedCommandHandler(
        IEquipmentAssignmentRepository assignmentRepository,
        ICurrentUserService currentUserService)
    {
        _assignmentRepository = assignmentRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(MarkAsDamagedCommand request, CancellationToken cancellationToken)
    {
        var assignment = await _assignmentRepository.GetByIdAsync(request.AssignmentId, cancellationToken);
        
        if (assignment == null)
            throw new Exception($"Assignment with ID {request.AssignmentId} not found");

        var currentUserId = _currentUserService.UserId;
        assignment.MarkAsDamaged(request.AdminNotes, currentUserId);

        _assignmentRepository.Update(assignment);
        await _assignmentRepository.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}







