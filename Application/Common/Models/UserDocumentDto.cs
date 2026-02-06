namespace Application.Common.Models;

public record UserDocumentDto(
    long Id,
    long UserId,
    long? ApplicationId,
    string FileName,
    string OriginalFileName,
    string FileType,
    long FileSize,
    DateTime CreatedAt,
    bool IsDeleted
);







