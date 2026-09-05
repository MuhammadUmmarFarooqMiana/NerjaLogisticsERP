namespace NerjaLogisticsERP.Application.Users.Queries;

public record UserListItemDto
{
    public Guid Id { get; init; }
    public string? Email { get; init; }
    public string? FullName { get; init; }
    public List<string> Roles { get; init; } = new();
}
