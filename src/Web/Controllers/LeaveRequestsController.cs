using Microsoft.AspNetCore.Mvc;
using NerjaLogisticsERP.Application.LeaveRequest.Commands.SubmitLeaveRequest;
using NerjaLogisticsERP.Application.LeaveRequest.Commands.ApproveLeaveRequest;
using NerjaLogisticsERP.Application.LeaveRequest.Commands.RejectLeaveRequest;

namespace NerjaLogisticsERP.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LeaveRequestsController : ApiControllerBase
{
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
