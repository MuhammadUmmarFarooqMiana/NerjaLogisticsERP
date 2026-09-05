namespace NerjaLogisticsERP.Application.DailyOrders.Commands.CorrectDailyOrder;

public class CorrectDailyOrderCommandValidator : AbstractValidator<CorrectDailyOrderCommand>
{
    public CorrectDailyOrderCommandValidator()
    {
        RuleFor(x => x.CompletedOrders).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(5000);
    }
}
