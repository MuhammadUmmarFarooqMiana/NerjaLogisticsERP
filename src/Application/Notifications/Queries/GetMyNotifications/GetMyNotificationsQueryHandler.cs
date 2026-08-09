using NerjaLogisticsERP.Application.Common.Interfaces;

namespace NerjaLogisticsERP.Application.Notifications.Queries.GetMyNotifications;

public class GetMyNotificationsQueryHandler : IRequestHandler<GetMyNotificationsQuery, List<NotificationDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _currentUser;

    public GetMyNotificationsQueryHandler(IApplicationDbContext context, IUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<NotificationDto>> Handle(GetMyNotificationsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Notifications.Where(n => n.UserId == _currentUser.Id);

        if (request.UnreadOnly == true)
            query = query.Where(n => !n.IsRead);

        return await query
            .OrderByDescending(n => n.Created)
            .Select(n => new NotificationDto(n.Id, n.Type, n.Title, n.Message, n.IsRead, n.Created))
            .ToListAsync(cancellationToken);
    }
}
