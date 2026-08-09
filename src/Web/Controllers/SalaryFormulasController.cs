using Microsoft.AspNetCore.Mvc;
using NerjaLogisticsERP.Application.Salaries.Commands.CreateSalaryFormula;
using NerjaLogisticsERP.Application.Salaries.Queries.CalculateSalary;
using NerjaLogisticsERP.Application.Salaries.Queries.GetSalaryFormulaById;
using NerjaLogisticsERP.Application.Salaries.Queries.GetSalaryFormulas;
using NerjaLogisticsERP.Application.Salaries.Commands.DeactivateSalaryFormula;

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
    public async Task<ActionResult<List<SalaryFormulaDto>>> GetAll([FromQuery] Guid? platformId, [FromQuery] bool activeOnly = false)
        => Ok(await Mediator.Send(new GetSalaryFormulasQuery { PlatformId = platformId, ActiveOnly = activeOnly }));

    [HttpGet("{id}")]
    public async Task<ActionResult<SalaryFormulaDto>> GetById(Guid id)
        => Ok(await Mediator.Send(new GetSalaryFormulaByIdQuery { Id = id }));

    [HttpPost("{id}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        await Mediator.Send(new DeactivateSalaryFormulaCommand { Id = id });
        return Ok(new { message = "Salary formula deactivated.", id });
    }
}
