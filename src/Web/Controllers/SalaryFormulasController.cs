using Microsoft.AspNetCore.Mvc;
using NerjaLogisticsERP.Application.Salaries.Commands.CreateSalaryFormula;
using NerjaLogisticsERP.Application.Salaries.Commands.DeactivateSalaryFormula;
using NerjaLogisticsERP.Application.Salaries.Commands.UpdateSalaryFormula;
using NerjaLogisticsERP.Application.Salaries.Queries.CalculateSalary;
using NerjaLogisticsERP.Application.Salaries.Queries.GetSalaryFormulaById;
using NerjaLogisticsERP.Application.Salaries.Queries.GetSalaryFormulas;

namespace NerjaLogisticsERP.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SalaryFormulasController : ApiControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateSalaryFormulaCommand command)
    {
        var id = await Mediator.Send(command);
        return Ok(new { message = "Salary formula created.", id });
    }

    [HttpGet("preview")]
    public async Task<ActionResult<SalaryPreviewDto>> Preview([FromQuery] Guid employeeId, [FromQuery] int year, [FromQuery] int month)
        => Ok(await Mediator.Send(new CalculateSalaryPreviewQuery { EmployeeId = employeeId, Year = year, Month = month }));

    [HttpGet]
    public async Task<ActionResult<List<SalaryFormulaDto>>> GetAll(
        [FromQuery] Guid? platformId, [FromQuery] bool activeOnly = false,
        [FromQuery] int? pageNumber = null, [FromQuery] int? pageSize = null)
    {
        var result = await Mediator.Send(new GetSalaryFormulasQuery
        {
            PlatformId = platformId,
            ActiveOnly = activeOnly,
            PageNumber = pageNumber,
            PageSize = pageSize
        });
        AddPaginationHeader(result);
        return Ok(result.Items);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SalaryFormulaDto>> GetById(Guid id)
        => Ok(await Mediator.Send(new GetSalaryFormulaByIdQuery { Id = id }));

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateSalaryFormulaCommand command)
    {
        if (id != command.Id) return BadRequest();
        await Mediator.Send(command);
        return Ok(new { message = "Salary formula updated.", id });
    }

    [HttpPost("{id}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        await Mediator.Send(new DeactivateSalaryFormulaCommand { Id = id });
        return Ok(new { message = "Salary formula deactivated.", id });
    }
}
