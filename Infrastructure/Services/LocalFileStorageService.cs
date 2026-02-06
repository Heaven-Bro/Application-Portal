namespace Infrastructure.Services;

using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _uploadBasePath;
    private readonly long _maxFileSizeBytes;
    private readonly HashSet<string> _allowedContentTypes;

    public LocalFileStorageService(IConfiguration configuration)
    {
        _uploadBasePath = configuration["FileStorage:UploadPath"] ?? "wwwroot/uploads";
        _maxFileSizeBytes = long.Parse(configuration["FileStorage:MaxFileSizeMB"] ?? "10") * 1024 * 1024;
        
        _allowedContentTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "application/pdf",
            "image/jpeg",
            "image/jpg",
            "image/png",
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
        };

        if (!Directory.Exists(_uploadBasePath))
        {
            Directory.CreateDirectory(_uploadBasePath);
        }
    }

    public async Task<(string fileName, string filePath)> SaveFileAsync(
        long userId,
        long? applicationId,
        Stream fileStream,
        string originalFileName,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        var userFolder = Path.Combine(_uploadBasePath, userId.ToString());
        var applicationsFolder = Path.Combine(userFolder, "applications");

        string targetFolder;
        if (applicationId.HasValue)
        {
            targetFolder = Path.Combine(applicationsFolder, applicationId.Value.ToString());
        }
        else
        {
            targetFolder = userFolder;
        }

        if (!Directory.Exists(targetFolder))
        {
            Directory.CreateDirectory(targetFolder);
        }

        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var fileExtension = Path.GetExtension(originalFileName);
        var fileName = $"{timestamp}_{Guid.NewGuid():N}{fileExtension}";
        var fullFilePath = Path.Combine(targetFolder, fileName);

        using (var fileStreamOutput = new FileStream(fullFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            await fileStream.CopyToAsync(fileStreamOutput, cancellationToken);
        }

        // Return web path
        var webPath = fullFilePath.Replace("wwwroot", "").Replace("\\", "/");
        
        return (fileName, webPath);
    }

    public Task<bool> DeleteFileAsync(string filePath, CancellationToken cancellationToken = default)
    {
        try
        {
            var physicalPath = filePath.StartsWith("/") 
                ? Path.Combine("wwwroot", filePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar))
                : filePath;

            if (File.Exists(physicalPath))
            {
                File.Delete(physicalPath);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    public Task<(Stream fileStream, string contentType, string fileName)?> GetFileAsync(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        var physicalPath = filePath.StartsWith("/") 
            ? Path.Combine("wwwroot", filePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar))
            : filePath;

        if (!File.Exists(physicalPath))
        {
            return Task.FromResult<(Stream, string, string)?>(null);
        }

        var fileStream = new FileStream(physicalPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        var fileName = Path.GetFileName(physicalPath);
        var extension = Path.GetExtension(physicalPath).ToLowerInvariant();

        var contentType = extension switch
        {
            ".pdf" => "application/pdf",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            _ => "application/octet-stream"
        };

        return Task.FromResult<(Stream, string, string)?>((fileStream, contentType, fileName));
    }

    public bool ValidateFileType(string contentType)
    {
        return _allowedContentTypes.Contains(contentType);
    }

    public bool ValidateFileSize(long fileSize)
    {
        return fileSize > 0 && fileSize <= _maxFileSizeBytes;
    }
}
