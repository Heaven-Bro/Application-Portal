namespace Application.Features.EquipmentAssignments.Commands;

using MediatR;
using FluentValidation;
using Domain.Repositories;
using Application.Common.Interfaces;

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
    private readonly ICurrentUserService _currentUserService;

    public RejectReturnCommandHandler(
        IEquipmentAssignmentRepository assignmentRepository,
        ICurrentUserService currentUserService)
    {
        _assignmentRepository = assignmentRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(RejectReturnCommand request, CancellationToken cancellationToken)
    {
        var assignment = await _assignmentRepository.GetByIdAsync(request.AssignmentId, cancellationToken);
        
        if (assignment == null)
            throw new Exception($"Assignment with ID {request.AssignmentId} not found");

        var currentUserId = _currentUserService.UserId;
        assignment.RejectReturn(request.AdminNotes, currentUserId);

        _assignmentRepository.Update(assignment);
        await _assignmentRepository.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}







