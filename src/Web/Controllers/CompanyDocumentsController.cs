using Microsoft.AspNetCore.Mvc;
using NerjaLogisticsERP.Application.Common.Models;
using NerjaLogisticsERP.Application.CompanyDocuments.Commands.CreateCompanyDocumentFolder;
using NerjaLogisticsERP.Application.CompanyDocuments.Commands.DeleteCompanyDocument;
using NerjaLogisticsERP.Application.CompanyDocuments.Commands.DeleteCompanyDocumentFolder;
using NerjaLogisticsERP.Application.CompanyDocuments.Commands.UploadCompanyDocument;
using NerjaLogisticsERP.Application.CompanyDocuments.Dtos;
using NerjaLogisticsERP.Application.CompanyDocuments.Queries.GetCompanyDocumentFolders;
using NerjaLogisticsERP.Application.CompanyDocuments.Queries.GetCompanyDocuments;
using NerjaLogisticsERP.Application.CompanyDocuments.Queries.GetEmployeeDocumentById;
using NerjaLogisticsERP.Domain.Enums;

namespace NerjaLogisticsERP.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CompanyDocumentsController : ApiControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Upload([FromForm] string title, [FromForm] CompanyDocumentCategory category, [FromForm] Guid? folderId, IFormFile file)
    {
        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);

        var id = await Mediator.Send(new UploadCompanyDocumentCommand
        {
            Title = title,
            Category = category,
            Content = ms.ToArray(),
            FileName = file.FileName,
            ContentType = file.ContentType,
            FolderId = folderId
        });

        return Ok(new { message = "Company document uploaded.", id });
    }

    [HttpGet]
    public async Task<ActionResult<List<CompanyDocumentDto>>> GetAll(
        [FromQuery] Guid? folderId, [FromQuery] int? pageNumber, [FromQuery] int? pageSize)
    {
        var result = await Mediator.Send(new GetCompanyDocumentsQuery { FolderId = folderId, PageNumber = pageNumber, PageSize = pageSize });
        AddPaginationHeader(result);
        return Ok(result.Items);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CompanyDocumentDto>> GetById(Guid id)
        => Ok(await Mediator.Send(new GetCompanyDocumentByIdQuery { Id = id }));

    [HttpGet("{id}/download")]
    public async Task<IActionResult> Download(Guid id)
    {
        var file = await Mediator.Send(new GetCompanyDocumentFileQuery { Id = id });
        return File(file.Content, file.ContentType, file.FileName);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await Mediator.Send(new DeleteCompanyDocumentCommand { Id = id });
        return Ok(new { message = "Company document deleted.", id });
    }

    [HttpGet("folders")]
    public async Task<ActionResult<List<CompanyDocumentFolderDto>>> GetFolders([FromQuery] Guid? parentFolderId)
        => Ok(await Mediator.Send(new GetCompanyDocumentFoldersQuery { ParentFolderId = parentFolderId }));

    [HttpPost("folders")]
    public async Task<IActionResult> CreateFolder(CreateCompanyDocumentFolderCommand command)
    {
        var id = await Mediator.Send(command);
        return Ok(new { message = "Folder created.", id });
    }

    [HttpDelete("folders/{id}")]
    public async Task<IActionResult> DeleteFolder(Guid id)
    {
        await Mediator.Send(new DeleteCompanyDocumentFolderCommand { Id = id });
        return Ok(new { message = "Folder deleted.", id });
    }
}
