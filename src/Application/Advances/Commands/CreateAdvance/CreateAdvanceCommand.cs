namespace NerjaLogisticsERP.Application.Advances.Commands.CreateAdvance;

public record CreateAdvanceCommand : IRequest<Guid>
{
    public Guid EmployeeId { get; init; }
    public decimal Amount { get; init; }
    public DateOnly AdvanceDate { get; init; }
    public string? Remarks { get; init; }
}
