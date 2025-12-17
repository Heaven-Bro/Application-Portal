namespace Application.Common.Models;

public record EquipmentDto(
    long Id,
    string Name,
    string EquipmentCode,
    string Category,
    bool IsAvailable
);

public record EquipmentAssignmentDto(
    long Id,
    long ApplicationId,
    long EquipmentId,
    string EquipmentName,
    DateTime AssignedAt,
    DateTime? ExpectedReturnDate,
    string Status,
    string? AdminNotes,
    string? ApplicantResponse
);
