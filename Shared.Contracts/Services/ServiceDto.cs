namespace Shared.Contracts.Services;

public record ServiceDto(
    long Id,
    string Name,
    string Description,
    bool IsActive,
    string ServiceType,
    List<ServiceStepDto> Steps
);

public record ServiceStepDto(
    long Id,
    string Name,
    int Order,
    bool RequiresFileUpload,
    bool RequiresTextInput,
    string? Instructions,
    string? DownloadableFormUrl,
    UploadConfigDto? UploadConfig
);

public record UploadConfigDto(
    string? Label,
    string? Instructions,
    string[]? AllowedTypes,
    int? MaxFiles
);
