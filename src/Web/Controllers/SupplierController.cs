using Microsoft.AspNetCore.Mvc;
using NerjaLogisticsERP.Application.Suppliers.Commands.CreateSupplier;
using NerjaLogisticsERP.Application.Suppliers.Commands.DeleteSupplier;
using NerjaLogisticsERP.Application.Suppliers.Commands.UpdateSupplier;
using NerjaLogisticsERP.Application.Suppliers.Queries.GetSupplierById;
using NerjaLogisticsERP.Application.Suppliers.Queries.GetSuppliers;

namespace NerjaLogisticsERP.Web.Controllers;

public class SuppliersController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<SupplierDto>>> GetAll([FromQuery] int? pageNumber, [FromQuery] int? pageSize)
    {
        var result = await Mediator.Send(new GetSuppliersQuery { PageNumber = pageNumber, PageSize = pageSize });
        AddPaginationHeader(result);
        return Ok(result.Items);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SupplierDto>> GetById(Guid id) => Ok(await Mediator.Send(new GetSupplierByIdQuery { Id = id }));

    [HttpPost]
    public async Task<IActionResult> Create(CreateSupplierCommand command)
    {
        var id = await Mediator.Send(command);
        return Ok(new { message = "Supplier created.", id });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateSupplierCommand command)
    {
        if (id != command.Id) return BadRequest();
        await Mediator.Send(command);
        return Ok(new { message = "Supplier updated.", id });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await Mediator.Send(new DeleteSupplierCommand { Id = id });
        return Ok(new { message = "Supplier deleted.", id });
    }
}
