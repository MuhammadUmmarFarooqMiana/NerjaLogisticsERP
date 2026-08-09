namespace NerjaLogisticsERP.Application.Fines.Commands.CreateFine;

public record CreateFineCommand : IRequest<Guid>
{
    public Guid EmployeeId { get; init; }
    public decimal Amount { get; init; }
    public string Reason { get; init; } = string.Empty;
    public DateOnly FineDate { get; init; }
}
