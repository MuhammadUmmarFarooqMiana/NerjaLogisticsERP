using Microsoft.AspNetCore.Mvc;
using NerjaLogisticsERP.Application.Employees.Commands.AdminCreateEmployee;
using NerjaLogisticsERP.Application.Employees.Commands.AdminUpdateEmployee;
using NerjaLogisticsERP.Application.Employees.Commands.ApproveEmployee;
using NerjaLogisticsERP.Application.Employees.Commands.DeleteEmployee;
using NerjaLogisticsERP.Application.Employees.Commands.ReactivateEmployee;
using NerjaLogisticsERP.Application.Employees.Commands.RejectEmployee;
using NerjaLogisticsERP.Application.Employees.Commands.RemoveMyProfilePicture;
using NerjaLogisticsERP.Application.Employees.Commands.SubmitProfileForReview;
using NerjaLogisticsERP.Application.Employees.Commands.SuspendEmployee;
using NerjaLogisticsERP.Application.Employees.Commands.TerminateEmployee;
using NerjaLogisticsERP.Application.Employees.Commands.UpdateEmployee;
using NerjaLogisticsERP.Application.Employees.Commands.UploadMyProfilePicture;
using NerjaLogisticsERP.Application.Employees.Queries;
using NerjaLogisticsERP.Application.Employees.Queries.GetEmployeeById;
using NerjaLogisticsERP.Application.Employees.Queries.GetEmployees;
using NerjaLogisticsERP.Application.Employees.Queries.GetMyProfile;
using NerjaLogisticsERP.Application.Employees.Queries.GetMyProfilePicture;
using NerjaLogisticsERP.Application.Employees.Queries.GetSupervisors;
using NerjaLogisticsERP.Domain.Enums;

namespace NerjaLogisticsERP.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmployeesController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<EmployeeListItemDto>>> GetEmployees(
        [FromQuery] string? status,
        [FromQuery] Guid? platformId,
        [FromQuery] string? searchTerm,
        [FromQuery] int? pageNumber,
        [FromQuery] int? pageSize)
    {
        // Plain string query param (not the AccountStatus enum type directly) so
        // the generated OpenAPI schema — and the frontend types built from it —
        // describe this as a string, matching every other enum-as-string on the
        // wire, instead of the numeric type ASP.NET Core would otherwise infer.
        AccountStatus? parsedStatus = Enum.TryParse<AccountStatus>(status, ignoreCase: true, out var result) ? result : null;

        var employees = await Mediator.Send(new GetEmployeesQuery
        {
            Status = parsedStatus,
            PlatformId = platformId,
            SearchTerm = searchTerm,
            PageNumber = pageNumber,
            PageSize = pageSize
        });
        AddPaginationHeader(employees);
        return Ok(employees.Items);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EmployeeDetailDto>> GetEmployeeById(Guid id)
        => Ok(await Mediator.Send(new GetEmployeeByIdQuery { Id = id }));

    [HttpGet("me")]
    public async Task<ActionResult<EmployeeDetailDto>> GetMyProfile()
        => Ok(await Mediator.Send(new GetMyProfileQuery()));

    [HttpGet("me/profile-picture")]
    public async Task<IActionResult> GetMyProfilePicture()
    {
        var file = await Mediator.Send(new GetMyProfilePictureQuery());
        // Null (not yet uploaded) is a plain 404 here, not an exception — see the
        // handler's comment for why that distinction matters.
        return file is null ? NotFound() : File(file.Content, file.ContentType);
    }

    [HttpPost("me/profile-picture")]
    public async Task<IActionResult> UploadMyProfilePicture(IFormFile file)
    {
        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);

        await Mediator.Send(new UploadMyProfilePictureCommand
        {
            Content = memoryStream.ToArray(),
            FileName = file.FileName,
            ContentType = file.ContentType
        });

        return Ok(new { message = "Profile picture updated." });
    }

    [HttpDelete("me/profile-picture")]
    public async Task<IActionResult> RemoveMyProfilePicture()
    {
        await Mediator.Send(new RemoveMyProfilePictureCommand());
        return Ok(new { message = "Profile picture removed." });
    }

    [HttpGet("supervisors")]
    public async Task<ActionResult<List<SupervisorLookupDto>>> GetSupervisors()
        => Ok(await Mediator.Send(new GetSupervisorsQuery()));

    [HttpPost("{id:guid}/submitProfile")]
    public async Task<IActionResult> SubmitProfile(Guid id, SubmitProfileForReviewCommand command)
    {
        if (id != command.UserId) return BadRequest();

        await Mediator.Send(command);
        return Ok(new { message = "Profile submitted for review.", employeeId = id });
    }

    [HttpPost("{id:guid}/approve")]
    public async Task<IActionResult> Approve(Guid id, [FromBody] ApproveEmployeeCommand command)
    {
        await Mediator.Send(command with { EmployeeId = id });
        return Ok(new { message = "Profile approved successfully.", employeeId = id });
    }

    [HttpPost("{id:guid}/reject")]
    public async Task<IActionResult> Reject(Guid id, [FromBody] RejectEmployeeCommand command)
    {
        await Mediator.Send(command with { EmployeeId = id });
        return Ok(new { message = "Profile rejected.", reason = command.Reason, employeeId = id });
    }

    [HttpPut("Id/UpdateProfile")]
    public async Task<IActionResult> UpdateProfile(UpdateEmployeeProfileCommand command)
    {
        await Mediator.Send(command);
        return Ok(new { message = "Profile Update." });
    }

    [HttpPost]
    public async Task<IActionResult> AdminCreate(AdminCreateEmployeeCommand command)
    {
        var id = await Mediator.Send(command);
        return Ok(new { message = "Employee created.", id });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> AdminUpdate(Guid id, AdminUpdateEmployeeCommand command)
    {
        if (id != command.Id) return BadRequest();
        await Mediator.Send(command);
        return Ok(new { message = "Employee updated.", id });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await Mediator.Send(new DeleteEmployeeCommand { Id = id });
        return Ok(new { message = "Employee deleted.", id });
    }

    [HttpPost("{id:guid}/suspend")]
    public async Task<IActionResult> Suspend(Guid id)
    {
        await Mediator.Send(new SuspendEmployeeCommand { Id = id });
        return Ok(new { message = "Employee suspended.", id });
    }

    [HttpPost("{id:guid}/reactivate")]
    public async Task<IActionResult> Reactivate(Guid id)
    {
        await Mediator.Send(new ReactivateEmployeeCommand { Id = id });
        return Ok(new { message = "Employee reactivated.", id });
    }

    [HttpPost("{id:guid}/terminate")]
    public async Task<IActionResult> Terminate(Guid id)
    {
        await Mediator.Send(new TerminateEmployeeCommand { Id = id });
        return Ok(new { message = "Employee terminated.", id });
    }
}
