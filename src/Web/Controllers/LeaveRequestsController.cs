using Microsoft.AspNetCore.Mvc;
using NerjaLogisticsERP.Application.LeaveRequest.Commands.ApproveLeaveRequest;
using NerjaLogisticsERP.Application.LeaveRequest.Commands.RejectLeaveRequest;
using NerjaLogisticsERP.Application.LeaveRequest.Commands.SubmitLeaveRequest;
using NerjaLogisticsERP.Application.LeaveRequest.Queries;
using NerjaLogisticsERP.Application.LeaveRequest.Queries.GetLeaveRequests;
using NerjaLogisticsERP.Application.LeaveRequest.Queries.GetMyLeaveRequests;
using NerjaLogisticsERP.Domain.Enums;

namespace NerjaLogisticsERP.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LeaveRequestsController : ApiControllerBase
{
    [HttpGet("me")]
    public async Task<ActionResult<List<LeaveRequestDto>>> GetMy([FromQuery] int? pageNumber, [FromQuery] int? pageSize)
    {
        var result = await Mediator.Send(new GetMyLeaveRequestsQuery { PageNumber = pageNumber, PageSize = pageSize });
        AddPaginationHeader(result);
        return Ok(result.Items);
    }

    [HttpGet]
    public async Task<ActionResult<List<LeaveRequestDto>>> GetAll(
        [FromQuery] LeaveStatus? status, [FromQuery] Guid? employeeId, [FromQuery] int? pageNumber, [FromQuery] int? pageSize)
    {
        var result = await Mediator.Send(new GetLeaveRequestsQuery
        {
            Status = status,
            EmployeeId = employeeId,
            PageNumber = pageNumber,
            PageSize = pageSize
        });
        AddPaginationHeader(result);
        return Ok(result.Items);
    }

    [HttpPost]
    public async Task<IActionResult> Submit(SubmitLeaveRequestCommand command)
    {
        var id = await Mediator.Send(command);
        return Ok(new { message = "Leave request submitted.", id });
    }

    [HttpPost("{id}/approve")]
    public async Task<IActionResult> Approve(Guid id)
    {
        await Mediator.Send(new ApproveLeaveRequestCommand { LeaveRequestId = id });
        return Ok(new { message = "Leave request approved.", id });
    }

    [HttpPost("{id}/reject")]
    public async Task<IActionResult> Reject(Guid id, [FromBody] string reason)
    {
        await Mediator.Send(new RejectLeaveRequestCommand { LeaveRequestId = id, Reason = reason });
        return Ok(new { message = "Leave request rejected.", id });
    }
}
