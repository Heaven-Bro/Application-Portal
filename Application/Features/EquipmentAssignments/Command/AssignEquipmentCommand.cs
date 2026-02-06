namespace Application.Features.EquipmentAssignments.Commands;

using MediatR;
using FluentValidation;
using Domain.Repositories;
using Domain.Equipment;
using Application.Common.Interfaces;

public sealed record AssignEquipmentCommand(
		long ApplicationId,
		long EquipmentId,
		DateTime? ExpectedReturnDate
) : IRequest<long>;

public sealed class AssignEquipmentCommandValidator : AbstractValidator<AssignEquipmentCommand>
{
	public AssignEquipmentCommandValidator()
	{
		RuleFor(x => x.ApplicationId)
				.GreaterThan(0).WithMessage("Application ID is required");

		RuleFor(x => x.EquipmentId)
				.GreaterThan(0).WithMessage("Equipment ID is required");

		RuleFor(x => x.ExpectedReturnDate)
				.Must(date => !date.HasValue || date.Value.Date >= DateTime.UtcNow.Date)
				.WithMessage("Expected return date must be today or in the future");
	}
}

public sealed class AssignEquipmentCommandHandler : IRequestHandler<AssignEquipmentCommand, long>
{
	private readonly IEquipmentRepository _equipmentRepository;
	private readonly IEquipmentAssignmentRepository _assignmentRepository;
	private readonly ICurrentUserService _currentUserService;

	public AssignEquipmentCommandHandler(
			IEquipmentRepository equipmentRepository,
			IEquipmentAssignmentRepository assignmentRepository,
			ICurrentUserService currentUserService)
	{
		_equipmentRepository = equipmentRepository;
		_assignmentRepository = assignmentRepository;
		_currentUserService = currentUserService;
	}

	public async Task<long> Handle(AssignEquipmentCommand request, CancellationToken cancellationToken)
	{
		var equipment = await _equipmentRepository.GetByIdAsync(request.EquipmentId, cancellationToken);

		if (equipment == null)
			throw new Exception($"Equipment with ID {request.EquipmentId} not found");

		if (!equipment.IsAvailable)
			throw new Exception("Equipment is not available for assignment");

		var currentUserId = _currentUserService.UserId;

		var assignment = EquipmentAssignment.Create(
				request.ApplicationId,
				request.EquipmentId,
				request.ExpectedReturnDate,
				currentUserId
		);

		equipment.MarkAsAssigned(currentUserId);

		await _assignmentRepository.AddAsync(assignment, cancellationToken);
		_equipmentRepository.Update(equipment);

		await _assignmentRepository.SaveChangesAsync(cancellationToken);

		return assignment.Id;
	}

}







