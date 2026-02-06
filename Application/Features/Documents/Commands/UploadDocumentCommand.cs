namespace Application.Features.Documents.Commands;

using Application.Common;
using Application.Common.Interfaces;
using Application.Common.Models;
using Shared.Contracts.Services;
using Shared.Contracts.Common;
using Domain.Documents;
using Domain.Repositories;
using FluentValidation;
using MediatR;

public record UploadDocumentCommand(
    long? ApplicationId,
    Stream FileStream,
    string OriginalFileName,
    string ContentType,
    long FileSize
) : IRequest<Common.Result<UserDocumentDto>>;

public class UploadDocumentCommandValidator : AbstractValidator<UploadDocumentCommand>
{
    private readonly IFileStorageService _fileStorageService;

    public UploadDocumentCommandValidator(IFileStorageService fileStorageService)
    {
        _fileStorageService = fileStorageService;

        RuleFor(x => x.OriginalFileName)
            .NotEmpty().WithMessage("File name is required")
            .MaximumLength(255).WithMessage("File name cannot exceed 255 characters");

        RuleFor(x => x.ContentType)
            .NotEmpty().WithMessage("Content type is required")
            .Must(ct => _fileStorageService.ValidateFileType(ct))
            .WithMessage("File type not allowed. Only PDF, JPG, PNG, DOCX are supported");

        RuleFor(x => x.FileSize)
            .GreaterThan(0).WithMessage("File size must be greater than zero")
            .Must(size => _fileStorageService.ValidateFileSize(size))
            .WithMessage("File size exceeds maximum allowed size (10MB)");

        RuleFor(x => x.FileStream)
            .NotNull().WithMessage("File stream is required");
    }
}

public class UploadDocumentCommandHandler : IRequestHandler<UploadDocumentCommand, Common.Result<UserDocumentDto>>
{
    private readonly IUserDocumentRepository _documentRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly ICurrentUserService _currentUserService;

    public UploadDocumentCommandHandler(
        IUserDocumentRepository documentRepository,
        IFileStorageService fileStorageService,
        ICurrentUserService currentUserService)
    {
        _documentRepository = documentRepository;
        _fileStorageService = fileStorageService;
        _currentUserService = currentUserService;
    }

    public async Task<Common.Result<UserDocumentDto>> Handle(UploadDocumentCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        (string fileName, string filePath) = await _fileStorageService.SaveFileAsync(
            currentUserId,
            request.ApplicationId,
            request.FileStream,
            request.OriginalFileName,
            request.ContentType,
            cancellationToken);

        var document = UserDocument.Create(
            currentUserId,
            request.ApplicationId,
            fileName,
            request.OriginalFileName,
            filePath,
            request.ContentType,
            request.FileSize,
            currentUserId);

        await _documentRepository.AddAsync(document, cancellationToken);
        await _documentRepository.SaveChangesAsync(cancellationToken);

        var dto = new UserDocumentDto(
            document.Id,
            document.UserId,
            document.ApplicationId,
            document.FileName,
            document.OriginalFileName,
            document.FileType,
            document.FileSize,
            document.CreatedAt,
            document.IsDeleted);

        return Common.Result<UserDocumentDto>.Success(dto);
    }
}







