namespace Application.Common.Interfaces;

public interface IFileStorageService
{
    Task<(string fileName, string filePath)> SaveFileAsync(
        long userId, 
        long? applicationId, 
        Stream fileStream, 
        string originalFileName, 
        string contentType,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteFileAsync(string filePath, CancellationToken cancellationToken = default);

    Task<(Stream fileStream, string contentType, string fileName)?> GetFileAsync(
        string filePath, 
        CancellationToken cancellationToken = default);

    bool ValidateFileType(string contentType);
    bool ValidateFileSize(long fileSize);
}







