using Microsoft.AspNetCore.Mvc;
using NerjaLogisticsERP.Application.Common.Models;
using NerjaLogisticsERP.Application.CompanyDocuments.Commands.UploadCompanyDocument;
using NerjaLogisticsERP.Application.CompanyDocuments.Dtos;
using NerjaLogisticsERP.Application.CompanyDocuments.Queries.GetCompanyDocuments;
using NerjaLogisticsERP.Application.CompanyDocuments.Queries.GetEmployeeDocumentById;
using NerjaLogisticsERP.Domain.Enums;

namespace NerjaLogisticsERP.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CompanyDocumentsController : ApiControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Upload([FromForm] string title, [FromForm] CompanyDocumentCategory category, IFormFile file)
    {
        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);

        var id = await Mediator.Send(new UploadCompanyDocumentCommand
        {
            Title = title,
            Category = category,
            Content = ms.ToArray(),
            FileName = file.FileName,
            ContentType = file.ContentType
        });

        return Ok(new { message = "Company document uploaded.", id });
    }
    [HttpGet]
    public async Task<ActionResult<List<CompanyDocumentDto>>> GetAll()
        => Ok(await Mediator.Send(new GetCompanyDocumentsQuery()));

    [HttpGet("{id}")]
    public async Task<ActionResult<CompanyDocumentDto>> GetById(Guid id)
        => Ok(await Mediator.Send(new GetCompanyDocumentByIdQuery { Id = id }));

    [HttpGet("{id}/download")]
    public async Task<IActionResult> Download(Guid id)
    {
        var file = await Mediator.Send(new GetCompanyDocumentFileQuery { Id = id });
        return File(file.Content, file.ContentType, file.FileName);
    }
}
