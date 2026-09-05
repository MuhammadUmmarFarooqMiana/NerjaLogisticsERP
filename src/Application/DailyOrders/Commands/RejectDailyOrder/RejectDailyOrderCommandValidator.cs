namespace NerjaLogisticsERP.Application.DailyOrders.Commands.RejectDailyOrder;

public class RejectDailyOrderCommandValidator : AbstractValidator<RejectDailyOrderCommand>
{
    public RejectDailyOrderCommandValidator()
    {
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(5000);
    }
}
