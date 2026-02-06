namespace Application.Features.Equipment.Commands;

using MediatR;
using FluentValidation;
using Domain.Equipment;
using Domain.Repositories;
using Application.Common.Interfaces;
using Domain.Common.Enums;

public sealed record CreateEquipmentCommand(
    string Name,
    string EquipmentCode,
    string Category,
    int Condition,
    string? Description
) : IRequest<long>;

public sealed class CreateEquipmentCommandValidator : AbstractValidator<CreateEquipmentCommand>
{
    public CreateEquipmentCommandValidator()
    {
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
            .InclusiveBetween(0, 4).WithMessage("Invalid condition value");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));
    }
}

public sealed class CreateEquipmentCommandHandler : IRequestHandler<CreateEquipmentCommand, long>
{
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly ICurrentUserService _currentUserService;

    public CreateEquipmentCommandHandler(
        IEquipmentRepository equipmentRepository,
        ICurrentUserService currentUserService)
    {
        _equipmentRepository = equipmentRepository;
        _currentUserService = currentUserService;
    }

    public async Task<long> Handle(CreateEquipmentCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        var equipment = Domain.Equipment.Equipment.Create(
            request.Name,
            request.EquipmentCode,
            request.Category,
            (EquipmentCondition)request.Condition,
            request.Description,
            currentUserId
        );

        await _equipmentRepository.AddAsync(equipment, cancellationToken);
        await _equipmentRepository.SaveChangesAsync(cancellationToken);

        return equipment.Id;
    }
}







