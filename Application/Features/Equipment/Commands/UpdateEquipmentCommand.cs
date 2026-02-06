namespace Application.Features.Equipment.Commands;

using MediatR;
using FluentValidation;
using Domain.Repositories;
using Application.Common.Interfaces;
using Domain.Common.Enums;

public sealed record UpdateEquipmentCommand(
    long Id,
    string Name,
    string EquipmentCode,
    string Category,
    int Condition,
    string? Description
) : IRequest;

public sealed class UpdateEquipmentCommandValidator : AbstractValidator<UpdateEquipmentCommand>
{
    public UpdateEquipmentCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Equipment ID is required");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Equipment name is required")
            .MaximumLength(255).WithMessage("Equipment name must not exceed 255 characters");

        RuleFor(x => x.EquipmentCode)
            .NotEmpty().WithMessage("Equipment code is required")
            .MaximumLength(100).WithMessage("Equipment code must not exceed 100 characters")
            .Matches(@"^[A-Z0-9\-_]+$").WithMessage("Equipment code must only contain uppercase letters, numbers, hyphens, and underscores");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Category is required")
            .MaximumLength(100).WithMessage("Category must not exceed 100 characters");

        RuleFor(x => x.Condition)
            .IsInEnum().WithMessage("Invalid condition value");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));
    }
}

public sealed class UpdateEquipmentCommandHandler : IRequestHandler<UpdateEquipmentCommand>
{
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly ICurrentUserService _currentUserService;

    public UpdateEquipmentCommandHandler(
        IEquipmentRepository equipmentRepository,
        ICurrentUserService currentUserService)
    {
        _equipmentRepository = equipmentRepository;
        _currentUserService = currentUserService;
    }

    public async Task Handle(UpdateEquipmentCommand request, CancellationToken cancellationToken)
    {
        var equipment = await _equipmentRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (equipment == null)
            throw new Exception($"Equipment with ID {request.Id} not found");

        var currentUserId = _currentUserService.UserId;

        equipment.UpdateDetails(
            request.Name, 
            request.EquipmentCode, 
            request.Category, 
            (EquipmentCondition)request.Condition, 
            request.Description, 
            currentUserId
        );

        _equipmentRepository.Update(equipment);
        await _equipmentRepository.SaveChangesAsync(cancellationToken);
    }
}







