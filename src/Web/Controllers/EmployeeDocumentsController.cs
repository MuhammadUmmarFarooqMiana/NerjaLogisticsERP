using Microsoft.AspNetCore.Mvc;
using NerjaLogisticsERP.Application.EmployeeDocument.Commands.UploadEmployeeDocument;
using NerjaLogisticsERP.Application.EmployeeDocument.Dtos;
using NerjaLogisticsERP.Application.EmployeeDocument.Queries.GetEmployeeDocumentById;
using NerjaLogisticsERP.Application.EmployeeDocument.Queries.GetEmployeeDocumentFile;
using NerjaLogisticsERP.Application.EmployeeDocument.Queries.GetEmployeeDocuments;
using NerjaLogisticsERP.Domain.Enums;

namespace NerjaLogisticsERP.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmployeeDocumentsController : ApiControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Upload([FromForm] Guid? employeeId, [FromForm] EmployeeDocumentType type, IFormFile file)
    {
        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);

        var id = await Mediator.Send(new UploadEmployeeDocumentCommand
        {
            EmployeeId = employeeId,
            Type = type,
            Content = memoryStream.ToArray(),
            FileName = file.FileName,
            ContentType = file.ContentType
        });

        return Ok(new { message = "Document uploaded.", id });
    }
    [HttpGet]
    public async Task<ActionResult<List<EmployeeDocumentDto>>> GetAll()
        => Ok(await Mediator.Send(new GetEmployeeDocumentsQuery()));

    [HttpGet("employee/{employeeId}")]
    public async Task<ActionResult<List<EmployeeDocumentDto>>> GetAllByEmployee(Guid employeeId)
        => Ok(await Mediator.Send(new GetEmployeeDocumentsQuery { EmployeeId = employeeId }));

    [HttpGet("platform/{platformId}")]
    public async Task<ActionResult<List<EmployeeDocumentDto>>> GetAllByPlatform(Guid platformId)
        => Ok(await Mediator.Send(new GetEmployeeDocumentsQuery { PlatformId = platformId }));

    [HttpGet("{id}")]
    public async Task<ActionResult<EmployeeDocumentDto>> GetById(Guid id)
        => Ok(await Mediator.Send(new GetEmployeeDocumentByIdQuery { Id = id }));

    [HttpGet("{id}/download")]
    public async Task<IActionResult> Download(Guid id)
    {
        var file = await Mediator.Send(new GetEmployeeDocumentFileQuery { Id = id });
        return File(file.Content, file.ContentType, file.FileName);
    }
}
