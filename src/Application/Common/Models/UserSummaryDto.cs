namespace NerjaLogisticsERP.Application.Common.Models;

/// <summary>Identity-side view of a user for the admin Users & Roles list — paired with Employee.FullName by the query handler.</summary>
public record UserSummaryDto
{
    public Guid Id { get; init; }
    public string? Email { get; init; }
    public IList<string> Roles { get; init; } = new List<string>();
}
