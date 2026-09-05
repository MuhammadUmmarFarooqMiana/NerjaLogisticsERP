namespace NerjaLogisticsERP.Application.DailyOrders.Commands.UpsertDailyOrder;

public class UpsertDailyOrderCommandValidator : AbstractValidator<UpsertDailyOrderCommand>
{
    public UpsertDailyOrderCommandValidator()
    {
        RuleFor(x => x.CompletedOrders).GreaterThanOrEqualTo(0);
    }
}
