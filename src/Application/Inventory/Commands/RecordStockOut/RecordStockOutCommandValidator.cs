namespace NerjaLogisticsERP.Application.Inventory.Commands.RecordStockOut;

public class RecordStockOutCommandValidator : AbstractValidator<RecordStockOutCommand>
{
    public RecordStockOutCommandValidator()
    {
        RuleFor(x => x.ItemId).NotEmpty();
        RuleFor(x => x.EmployeeId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}
