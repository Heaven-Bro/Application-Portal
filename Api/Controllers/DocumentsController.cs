namespace Api.Controllers;

using Application.Common.Interfaces;
using Application.Features.Documents.Commands;
using Application.Features.Documents.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DocumentController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IFileStorageService _fileStorageService;

    public DocumentController(IMediator mediator, IFileStorageService fileStorageService)
    {
        _mediator = mediator;
        _fileStorageService = fileStorageService;
    }

    [HttpPost("upload")]
    [RequestSizeLimit(20 * 1024 * 1024)]
    [RequestFormLimits(MultipartBodyLengthLimit = 20 * 1024 * 1024)]
    public async Task<IActionResult> UploadDocument(
        [FromForm] IFormFile file,
        [FromForm] long? applicationId = null)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { message = "No file uploaded" });

        await using var stream = file.OpenReadStream();

        var command = new UploadDocumentCommand(
            applicationId,
            stream,
            file.FileName,
            file.ContentType,
            file.Length);

        var result = await _mediator.Send(command);

        if (!result.IsSuccess || result.Value == null)
            return BadRequest(new { message = result.Error ?? "Upload failed" });

        return Ok(new { documentId = result.Value.Id });
    }

    [HttpGet]
    public async Task<IActionResult> GetMyDocuments([FromQuery] long? applicationId = null)
    {
        var query = new GetUserDocumentsQuery(applicationId);
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });

        return Ok(result.Value);
    }

    [HttpGet("{id}/download")]
    public async Task<IActionResult> DownloadDocument(long id)
    {
        var query = new GetUserDocumentsQuery();
        var result = await _mediator.Send(query);

        if (!result.IsSuccess || result.Value == null)
            return BadRequest(new { message = result.Error ?? "Failed to retrieve documents" });

        var document = result.Value.FirstOrDefault(d => d.Id == id);
        if (document == null)
            return NotFound(new { message = "Document not found" });

        var fileResult = await _fileStorageService.GetFileAsync(document.FileName);

        if (fileResult == null)
            return NotFound(new { message = "File not found on disk" });

        return File(fileResult.Value.fileStream, fileResult.Value.contentType, document.OriginalFileName);
    }
}
