using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.DailyOrders.Queries;

namespace NerjaLogisticsERP.Application.DailyOrders.Queries.GetMyDailyOrder;

public class GetMyDailyOrderQueryHandler : IRequestHandler<GetMyDailyOrderQuery, DailyOrderDto?>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public GetMyDailyOrderQueryHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task<DailyOrderDto?> Handle(GetMyDailyOrderQuery request, CancellationToken cancellationToken)
    {
        var userId = _user.Id!.Value;
        var today = DailyOrderClock.Today();

        return await _context.DailyOrders
            .Where(o => o.Employee.UserId == userId && o.OrderDate == today)
            .Select(o => new DailyOrderDto
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                CompletedOrders = o.CompletedOrders,
                Status = o.Status.ToString(),
                ClosedAt = o.ClosedAt,
                ReviewNote = o.ReviewNote
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}
