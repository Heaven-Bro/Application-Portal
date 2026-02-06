namespace Application.Features.Equipment.Commands;

using MediatR;
using FluentValidation;
using Domain.Repositories;
using Application.Common.Interfaces;

public sealed record DeleteEquipmentCommand(long Id) : IRequest;

public sealed class DeleteEquipmentCommandValidator : AbstractValidator<DeleteEquipmentCommand>
{
    public DeleteEquipmentCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Equipment ID is required");
    }
}

public sealed class DeleteEquipmentCommandHandler : IRequestHandler<DeleteEquipmentCommand>
{
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly IEquipmentAssignmentRepository _assignmentRepository;

    public DeleteEquipmentCommandHandler(
        IEquipmentRepository equipmentRepository,
        IEquipmentAssignmentRepository assignmentRepository)
    {
        _equipmentRepository = equipmentRepository;
        _assignmentRepository = assignmentRepository;
    }

    public async Task Handle(DeleteEquipmentCommand request, CancellationToken cancellationToken)
    {
        var equipment = await _equipmentRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (equipment == null)
            throw new Exception($"Equipment with ID {request.Id} not found");

        var assignments = await _assignmentRepository.GetByEquipmentIdAsync(request.Id, cancellationToken);
        
        if (assignments.Any())
            throw new Exception("Cannot delete equipment with existing assignments");

        _equipmentRepository.Delete(equipment);
        await _equipmentRepository.SaveChangesAsync(cancellationToken);
    }
}







