namespace Application.Features.Documents.Queries;

using Application.Common;
using Application.Common.Interfaces;
using Application.Common.Models;
using Shared.Contracts.Services;
using Shared.Contracts.Common;
using Domain.Repositories;
using MediatR;

public record GetUserDocumentsQuery(
    long? ApplicationId = null
) : IRequest<Common.Result<List<UserDocumentDto>>>;

public class GetUserDocumentsQueryHandler : IRequestHandler<GetUserDocumentsQuery, Common.Result<List<UserDocumentDto>>>
{
    private readonly IUserDocumentRepository _documentRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetUserDocumentsQueryHandler(
        IUserDocumentRepository documentRepository,
        ICurrentUserService currentUserService)
    {
        _documentRepository = documentRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Common.Result<List<UserDocumentDto>>> Handle(GetUserDocumentsQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        var documents = request.ApplicationId.HasValue
            ? await _documentRepository.GetByApplicationIdAsync(request.ApplicationId.Value, cancellationToken)
            : await _documentRepository.GetByUserIdAsync(currentUserId, includeDeleted: false, cancellationToken);

        var dtos = documents
            .Where(d => d.UserId == currentUserId)
            .Select(d => new UserDocumentDto(
                d.Id,
                d.UserId,
                d.ApplicationId,
                d.FileName,
                d.OriginalFileName,
                d.FileType,
                d.FileSize,
                d.CreatedAt,
                d.IsDeleted))
            .ToList();

        return Common.Result<List<UserDocumentDto>>.Success(dtos);
    }
}







