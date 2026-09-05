namespace NerjaLogisticsERP.Application.Platforms.Queries.GetPlatforms;

public record PlatformDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
}
