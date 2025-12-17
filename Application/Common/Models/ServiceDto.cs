namespace Application.Common.Models;

public record ServiceDto(
    long Id,
    string Name,
    string Description,
    bool IsActive,
    int ServiceVersion,
    List<ServiceStepDto> Steps
);

public record ServiceStepDto(
    long Id,
    string Name,
    string Description,
    int Order,
    bool RequiresFileUpload,
    bool RequiresTextInput,
    bool IsEquipmentAssignment
);
