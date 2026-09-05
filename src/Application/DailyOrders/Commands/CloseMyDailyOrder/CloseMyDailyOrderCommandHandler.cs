using NerjaLogisticsERP.Application.Common.Exceptions;
using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.DailyOrders;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.DailyOrders.Commands.CloseMyDailyOrder;

public class CloseMyDailyOrderCommandHandler : IRequestHandler<CloseMyDailyOrderCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public CloseMyDailyOrderCommandHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task Handle(CloseMyDailyOrderCommand request, CancellationToken cancellationToken)
    {
        var userId = _user.Id!.Value;
        var today = DailyOrderClock.Today();

        var order = await _context.DailyOrders
            .FirstOrDefaultAsync(o => o.Employee.UserId == userId && o.OrderDate == today, cancellationToken)
            ?? throw new NotFoundException(nameof(DailyOrder), today.ToString());

        order.Close();
        await _context.SaveChangesAsync(cancellationToken);
    }
}
