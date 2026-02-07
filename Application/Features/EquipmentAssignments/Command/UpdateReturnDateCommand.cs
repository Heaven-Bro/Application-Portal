namespace Application.Features.EquipmentAssignments.Commands;

using MediatR;
using FluentValidation;
using Domain.Repositories;
using Application.Common.Interfaces;

public sealed record UpdateReturnDateCommand(
    long AssignmentId,
    DateTime? ExpectedReturnDate
) : IRequest<Unit>;

public sealed class UpdateReturnDateCommandValidator : AbstractValidator<UpdateReturnDateCommand>
{
    public UpdateReturnDateCommandValidator()
    {
        RuleFor(x => x.AssignmentId)
            .GreaterThan(0).WithMessage("Assignment ID is required");

        RuleFor(x => x.ExpectedReturnDate)
            .Must(date => !date.HasValue || date.Value.Date >= DateTime.UtcNow.Date)
            .WithMessage("Expected return date must be today or in the future");
    }
}

public sealed class UpdateReturnDateCommandHandler : IRequestHandler<UpdateReturnDateCommand, Unit>
{
    private readonly IEquipmentAssignmentRepository _assignmentRepository;
    private readonly ICurrentUserService _currentUserService;

    public UpdateReturnDateCommandHandler(
        IEquipmentAssignmentRepository assignmentRepository,
        ICurrentUserService currentUserService)
    {
        _assignmentRepository = assignmentRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(UpdateReturnDateCommand request, CancellationToken cancellationToken)
    {
        var assignment = await _assignmentRepository.GetByIdAsync(request.AssignmentId, cancellationToken);

        if (assignment == null)
            throw new Exception($"Assignment with ID {request.AssignmentId} not found");

        var currentUserId = _currentUserService.UserId;
        assignment.UpdateExpectedReturnDate(request.ExpectedReturnDate, currentUserId);

        _assignmentRepository.Update(assignment);
        await _assignmentRepository.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
