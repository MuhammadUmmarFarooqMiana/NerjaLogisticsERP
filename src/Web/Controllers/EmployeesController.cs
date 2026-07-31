using Microsoft.AspNetCore.Mvc;
using NerjaLogisticsERP.Application.Employees.Commands.ApproveEmployee;
using NerjaLogisticsERP.Application.Employees.Commands.RejectEmployee;
using NerjaLogisticsERP.Application.Employees.Commands.SubmitProfileForReview;
using NerjaLogisticsERP.Application.Employees.Commands.UpdateEmployee;

namespace NerjaLogisticsERP.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmployeesController : ApiControllerBase
{
    [HttpPost("{id}/submitProfile")]
    public async Task<IActionResult> SubmitProfile(Guid id, SubmitProfileForReviewCommand command)
    {
        if (id != command.EmployeeId) return BadRequest();

        await Mediator.Send(new SubmitEmployeeProfileForReviewCommand());
        return Ok(new { message = "Profile submitted for review.", employeeId = id });
    }

    [HttpPost("{id}/approve")]
    public async Task<IActionResult> Approve(Guid id, [FromBody] DateOnly? joiningDate)
    {
        await Mediator.Send(new ApproveEmployeeCommand { EmployeeId = id, JoiningDate = joiningDate });
        return Ok(new { message = "Profile approved successfully.", employeeId = id });
    }

    [HttpPost("{id}/reject")]
    public async Task<IActionResult> Reject(Guid id, [FromBody] string reason)
    {
        await Mediator.Send(new RejectEmployeeCommand { EmployeeId = id, Reason = reason });
        return Ok(new { message = "Profile rejected.", reason = reason, employeeId = id });
    }

    [HttpPut("Id/UpdateProfile")]
    public async Task<IActionResult> UpdateProfile(UpdateEmployeeProfileCommand command)
    {
        await Mediator.Send(command);
        return Ok(new { message = "Profile Update." });
    }

}
