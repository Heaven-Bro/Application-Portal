namespace Application.Features.EquipmentAssignments.Commands;

using MediatR;
using FluentValidation;
using Domain.Repositories;
using Application.Common.Interfaces;

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
    private readonly ICurrentUserService _currentUserService;

    public DisputeDamageCommandHandler(
        IEquipmentAssignmentRepository assignmentRepository,
        ICurrentUserService currentUserService)
    {
        _assignmentRepository = assignmentRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(DisputeDamageCommand request, CancellationToken cancellationToken)
    {
        var assignment = await _assignmentRepository.GetByIdAsync(request.AssignmentId, cancellationToken);
        
        if (assignment == null)
            throw new Exception($"Assignment with ID {request.AssignmentId} not found");

        var currentUserId = _currentUserService.UserId;
        assignment.DisputeDamage(request.ApplicantResponse, currentUserId);

        _assignmentRepository.Update(assignment);
        await _assignmentRepository.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}







