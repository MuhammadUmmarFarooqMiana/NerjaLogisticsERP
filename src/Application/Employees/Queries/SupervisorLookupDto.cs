namespace NerjaLogisticsERP.Application.Employees.Queries;

public record SupervisorLookupDto
{
    public Guid Id { get; init; }
    public string FullName { get; init; } = default!;
}
