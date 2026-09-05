namespace NerjaLogisticsERP.Application.Inventory.Commands.RecordStockIn;

public class RecordStockInCommandValidator : AbstractValidator<RecordStockInCommand>
{
    public RecordStockInCommandValidator()
    {
        RuleFor(x => x.ItemId).NotEmpty();
        RuleFor(x => x.SupplierId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}
