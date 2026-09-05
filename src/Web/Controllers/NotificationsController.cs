using Microsoft.AspNetCore.Mvc;
using NerjaLogisticsERP.Application.Notifications.Commands.MarkNotificationAsRead;
using NerjaLogisticsERP.Application.Notifications.Queries.GetMyNotifications;

namespace NerjaLogisticsERP.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NotificationsController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<NotificationDto>>> GetMine([FromQuery] bool? unreadOnly)
        => Ok(await Mediator.Send(new GetMyNotificationsQuery { UnreadOnly = unreadOnly }));

    [HttpPost("{id}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        await Mediator.Send(new MarkNotificationAsReadCommand { Id = id });
        return Ok(new { message = "Notification marked as read.", id });
    }
}
